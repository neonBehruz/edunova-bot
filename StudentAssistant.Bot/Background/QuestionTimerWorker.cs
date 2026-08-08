using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudentAssistant.Bot.Handlers;
using StudentAssistant.Bot.State;
using StudentAssistant.Service.DTOs.Tests;
using Telegram.Bot;

namespace StudentAssistant.Bot.Background;

public class QuestionTimerWorker : BackgroundService
{
    private readonly TestSessionManager _sessionManager;
    private readonly ITelegramBotClient _botClient;
    private readonly QuestionHandler _questionHandler;
    private readonly ResultHandler _resultHandler;
    private readonly ILogger<QuestionTimerWorker> _logger;

    public QuestionTimerWorker(
        TestSessionManager sessionManager,
        ITelegramBotClient botClient,
        QuestionHandler questionHandler,
        ResultHandler resultHandler,
        ILogger<QuestionTimerWorker> logger)
    {
        _sessionManager = sessionManager;
        _botClient = botClient;
        _questionHandler = questionHandler;
        _resultHandler = resultHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("QuestionTimerWorker background service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var activeSessions = _sessionManager.GetAllActiveSessions().ToList();
                var now = DateTime.UtcNow;

                foreach (var session in activeSessions)
                {
                    // Check if current question timed out (60 seconds)
                    if ((now - session.QuestionStartedAt).TotalSeconds >= 60)
                    {
                        var currentQuestion = session.Questions[session.CurrentQuestionIndex];

                        // Auto-record unanswered question
                        session.UserAnswers.Add(new SubmitAnswerDto
                        {
                            AttemptId = session.AttemptId,
                            QuestionId = currentQuestion.Id,
                            SelectedOptionId = null,
                            TimeTakenSeconds = 60
                        });

                        // Notify user on timeout
                        try
                        {
                            await _botClient.SendMessage(
                                chatId: session.TelegramChatId,
                                text: $"⏱️ *Vaqt tugadi!* ({session.CurrentQuestionIndex + 1}-savol uchun 60 sekund o'tdi). Keyingi savolga o'tildi.",
                                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                                cancellationToken: stoppingToken);
                        }
                        catch { /* Ignore network errors */ }

                        session.CurrentQuestionIndex++;

                        if (session.CurrentQuestionIndex < session.Questions.Count)
                        {
                            await _questionHandler.SendCurrentQuestionAsync(_botClient, session, stoppingToken);
                        }
                        else
                        {
                            await _resultHandler.FinalizeAndSendResultAsync(_botClient, session, stoppingToken);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in QuestionTimerWorker loop");
            }

            await Task.Delay(2000, stoppingToken);
        }
    }
}
