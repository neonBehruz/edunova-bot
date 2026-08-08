using StudentAssistant.Bot.Keyboards;
using StudentAssistant.Bot.State;
using StudentAssistant.Service.DTOs.Tests;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentAssistant.Bot.Handlers;

public class AnswerHandler
{
    private readonly TestSessionManager _sessionManager;
    private readonly QuestionHandler _questionHandler;
    private readonly ResultHandler _resultHandler;

    public AnswerHandler(
        TestSessionManager sessionManager,
        QuestionHandler questionHandler,
        ResultHandler resultHandler)
    {
        _sessionManager = sessionManager;
        _questionHandler = questionHandler;
        _resultHandler = resultHandler;
    }

    public async Task HandleCallbackAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        if (callbackQuery.Data == null) return;

        long userId = callbackQuery.From.Id;

        // Handle End Test button
        if (callbackQuery.Data.StartsWith("finish_"))
        {
            var activeSession = _sessionManager.GetSession(userId);
            if (activeSession != null)
            {
                await botClient.AnswerCallbackQuery(callbackQuery.Id, "🛑 Test yakunlandi.", cancellationToken: cancellationToken);
                await _resultHandler.FinalizeAndSendResultAsync(botClient, activeSession, cancellationToken);
            }
            else
            {
                await botClient.AnswerCallbackQuery(callbackQuery.Id, "❌ Ushbu test sessiyasi yakunlangan.", cancellationToken: cancellationToken);
            }
            return;
        }

        if (!callbackQuery.Data.StartsWith("ans_")) return;

        var session = _sessionManager.GetSession(userId);
        if (session == null)
        {
            await botClient.AnswerCallbackQuery(callbackQuery.Id, "❌ Ushbu test sessiyasi yakunlangan.", cancellationToken: cancellationToken);
            return;
        }

        // Format: ans_{attemptId}_{questionId}_{optionId}
        var parts = callbackQuery.Data.Split('_');
        if (parts.Length < 4) return;

        long questionId = long.Parse(parts[2]);
        long optionId = long.Parse(parts[3]);

        var currentQuestion = session.Questions[session.CurrentQuestionIndex];
        if (currentQuestion.Id != questionId)
        {
            await botClient.AnswerCallbackQuery(callbackQuery.Id, "⚠️ Eski savolga javob berib bo'lmaydi.", cancellationToken: cancellationToken);
            return;
        }

        int timeTaken = (int)(DateTime.UtcNow - session.QuestionStartedAt).TotalSeconds;

        session.UserAnswers.Add(new SubmitAnswerDto
        {
            AttemptId = session.AttemptId,
            QuestionId = questionId,
            SelectedOptionId = optionId,
            TimeTakenSeconds = timeTaken
        });

        await botClient.AnswerCallbackQuery(callbackQuery.Id, "✅ Javobingiz qabul qilindi!", cancellationToken: cancellationToken);

        // Edit question message to show selected option with checkmark
        if (callbackQuery.Message != null)
        {
            try
            {
                var answeredKeyboard = AnswerKeyboard.GetAnsweredKeyboard(currentQuestion.Options.ToList(), optionId);
                await botClient.EditMessageReplyMarkup(
                    chatId: callbackQuery.Message.Chat.Id,
                    messageId: callbackQuery.Message.MessageId,
                    replyMarkup: answeredKeyboard,
                    cancellationToken: cancellationToken);
            }
            catch { /* Ignore edit errors */ }
        }

        // Advance question
        session.CurrentQuestionIndex++;

        if (session.CurrentQuestionIndex < session.Questions.Count)
        {
            await _questionHandler.SendCurrentQuestionAsync(botClient, session, cancellationToken);
        }
        else
        {
            // Test completed!
            await _resultHandler.FinalizeAndSendResultAsync(botClient, session, cancellationToken);
        }
    }
}
