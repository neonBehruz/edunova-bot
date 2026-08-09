using StudentAssistant.Bot.Keyboards;
using StudentAssistant.Bot.State;
using StudentAssistant.Domain.Entities;
using StudentAssistant.Domain.Enums;
using StudentAssistant.Service.DTOs.Tests;
using StudentAssistant.Service.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentAssistant.Bot.Handlers;

public class TestHandler
{
    private readonly TestSessionManager _sessionManager;
    private readonly IQuestionGeneratorService _generatorService;
    private readonly ITestAttemptService _attemptService;
    private readonly QuestionHandler _questionHandler;

    public TestHandler(
        TestSessionManager sessionManager,
        IQuestionGeneratorService generatorService,
        ITestAttemptService attemptService,
        QuestionHandler questionHandler)
    {
        _sessionManager = sessionManager;
        _generatorService = generatorService;
        _attemptService = attemptService;
        _questionHandler = questionHandler;
    }

    public async Task HandleQuestionCountPromptAsync(ITelegramBotClient botClient, Message message, long telegramId, DifficultyLevel difficulty, CancellationToken cancellationToken)
    {
        _sessionManager.SetUserState(telegramId, UserStateStep.SelectingQuestionCount);
        _sessionManager.SetUserDifficultySelection(telegramId, difficulty);

        string text = "🔢 *Savollar sonini tanlang:*\n\n" +
                      "⏱️ Har bir test jarayoni uchun savollarga javob berish taymeri ishlaydi.";

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: text,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: QuestionCountKeyboard.GetKeyboard(),
            cancellationToken: cancellationToken);
    }

    public async Task StartTestAsync(ITelegramBotClient botClient, Message message, StudentAssistant.Service.DTOs.Users.UserDto user, int count, CancellationToken cancellationToken)
    {
        long telegramId = message.From!.Id;
        var selections = _sessionManager.GetUserSelections(telegramId);
        var level = selections.Level ?? CefrLevel.A1;
        var difficulty = selections.Difficulty ?? DifficultyLevel.Easy;
        long subjectId = selections.SubjectId ?? 1;

        // 1. Generate Questions
        var questions = await _generatorService.GenerateQuestionsAsync(user.Id, subjectId, level, difficulty, count);

        if (!questions.Any())
        {
            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: "❌ Afsuski, bu daraja va qiyinchilik uchun hozircha savollar mavjud emas.",
                replyMarkup: MainMenuKeyboard.GetKeyboard(),
                cancellationToken: cancellationToken);
            _sessionManager.SetUserState(telegramId, UserStateStep.MainMenu);
            return;
        }

        // 2. Create Test Attempt record in database
        var startDto = new StartTestDto
        {
            UserId = user.Id,
            SubjectId = subjectId,
            Level = level,
            Difficulty = difficulty,
            QuestionCount = questions.Count
        };

        var attempt = await _attemptService.CreateAttemptAsync(startDto, questions.Select(q => q.Id).ToList());

        // 3. Initialize active TestSession indexed by TelegramId
        var session = new TestSession
        {
            UserId = telegramId, // Telegram user ID matching callbackQuery.From.Id
            TelegramChatId = message.Chat.Id,
            AttemptId = attempt.Id,
            SelectedLevel = level,
            SubjectId = subjectId,
            SubjectName = selections.SubjectName,
            SelectedDifficulty = difficulty,
            RequestedQuestionCount = questions.Count,
            Questions = questions,
            CurrentQuestionIndex = 0,
            QuestionStartedAt = DateTime.UtcNow,
            SessionExpiresAt = DateTime.UtcNow.AddSeconds(questions.Count * 60 + 30) // max overall timeout
        };

        _sessionManager.StartSession(session);
        _sessionManager.SetUserState(telegramId, UserStateStep.InTest);

        string startMessageText = "🎯 *TEST BOSHLANDI*\n\n" +
                                 $"📖 *Fan:* {selections.SubjectName}\n" +
                                 $"📝 *Savollar:* {questions.Count} ta\n" +
                                 "⏳ *Har bir savol uchun:* 60 soniya\n\n" +
                                 "🚀 *Boshladik!*\n" +
                                 "Omad yor bo‘lsin! 🍀";

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: startMessageText,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: cancellationToken);

        // Send first question
        await _questionHandler.SendCurrentQuestionAsync(botClient, session, cancellationToken);
    }
}
