using StudentAssistant.Bot.Keyboards;
using StudentAssistant.Bot.State;
using StudentAssistant.Domain.Enums;
using StudentAssistant.Service.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentAssistant.Bot.Handlers;

public class MainMenuHandler
{
    private readonly TestSessionManager _sessionManager;
    private readonly StartHandler _startHandler;
    private readonly LevelHandler _levelHandler;
    private readonly SubjectHandler _subjectHandler;
    private readonly DifficultyHandler _difficultyHandler;
    private readonly TestHandler _testHandler;
    private readonly ResultHandler _resultHandler;
    private readonly RatingHandler _ratingHandler;
    private readonly AboutHandler _aboutHandler;
    private readonly SupportHandler _supportHandler;
    private readonly IUserService _userService;

    public MainMenuHandler(
        TestSessionManager sessionManager,
        StartHandler startHandler,
        LevelHandler levelHandler,
        SubjectHandler subjectHandler,
        DifficultyHandler difficultyHandler,
        TestHandler testHandler,
        ResultHandler resultHandler,
        RatingHandler ratingHandler,
        AboutHandler aboutHandler,
        SupportHandler supportHandler,
        IUserService userService)
    {
        _sessionManager = sessionManager;
        _startHandler = startHandler;
        _levelHandler = levelHandler;
        _subjectHandler = subjectHandler;
        _difficultyHandler = difficultyHandler;
        _testHandler = testHandler;
        _resultHandler = resultHandler;
        _ratingHandler = ratingHandler;
        _aboutHandler = aboutHandler;
        _supportHandler = supportHandler;
        _userService = userService;
    }

    public async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        if (message.From == null || string.IsNullOrWhiteSpace(message.Text)) return;

        long telegramId = message.From.Id;

        var user = await _userService.GetOrCreateUserAsync(
            telegramId,
            message.From.FirstName,
            message.From.LastName,
            message.From.Username);

        string text = message.Text.Trim();
        var currentStep = _sessionManager.GetUserState(telegramId);

        // Global Command Override
        if (text == "/start" || text == "🏠 Bosh menyu")
        {
            _sessionManager.RemoveSession(telegramId);
            _sessionManager.SetUserState(telegramId, UserStateStep.MainMenu);
            await _startHandler.HandleAsync(botClient, message, cancellationToken);
            return;
        }

        // Back button handler
        if (text == "⬅️ Orqaga")
        {
            switch (currentStep)
            {
                case UserStateStep.SelectingLevel:
                    _sessionManager.SetUserState(telegramId, UserStateStep.MainMenu);
                    await _startHandler.HandleAsync(botClient, message, cancellationToken);
                    break;

                case UserStateStep.SelectingSubject:
                    await _levelHandler.HandlePromptAsync(botClient, message, telegramId, cancellationToken);
                    break;

                case UserStateStep.SelectingDifficulty:
                    await _subjectHandler.HandlePromptAsync(botClient, message, telegramId, cancellationToken);
                    break;

                default:
                    _sessionManager.SetUserState(telegramId, UserStateStep.MainMenu);
                    await _startHandler.HandleAsync(botClient, message, cancellationToken);
                    break;
            }
            return;
        }

        // Handle Main Menu Navigation
        if (text == "🎯 Test boshlash" || text == "🎯 Yangi test boshlash")
        {
            await _levelHandler.HandlePromptAsync(botClient, message, telegramId, cancellationToken);
            return;
        }

        if (text == "📊 Natijalarim")
        {
            await _resultHandler.SendUserHistoryAsync(botClient, message, user.Id, cancellationToken);
            return;
        }

        if (text == "🏆 Reyting")
        {
            await _ratingHandler.SendLeaderboardAsync(botClient, message, user.Id, cancellationToken);
            return;
        }

        if (text == "ℹ️ Bot haqida")
        {
            await _aboutHandler.HandleAsync(botClient, message, cancellationToken);
            return;
        }

        if (text == "🆘 Support")
        {
            await _supportHandler.HandleAsync(botClient, message, cancellationToken);
            return;
        }

        // Handle Step-Specific Inputs
        switch (currentStep)
        {
            case UserStateStep.SelectingLevel:
                if (_levelHandler.TryParseLevel(text, out var level, out var grade))
                {
                    _sessionManager.SetUserLevelSelection(telegramId, level, grade);
                    await _subjectHandler.HandlePromptAsync(botClient, message, telegramId, cancellationToken);
                }
                else
                {
                    await botClient.SendMessage(message.Chat.Id, "⚠️ Iltimos, pastdagi sinflardan birini tanlang!", replyMarkup: LevelKeyboard.GetKeyboard(), cancellationToken: cancellationToken);
                }
                break;

            case UserStateStep.SelectingSubject:
                if (_subjectHandler.TryParseSubject(text, out var subjectId, out var subjectName))
                {
                    _sessionManager.SetUserSubjectSelection(telegramId, subjectId, subjectName);
                    var sel = _sessionManager.GetUserSelections(telegramId);
                    await _difficultyHandler.HandlePromptAsync(botClient, message, telegramId, sel.Level ?? CefrLevel.A1, cancellationToken);
                }
                else
                {
                    var sel = _sessionManager.GetUserSelections(telegramId);
                    await botClient.SendMessage(message.Chat.Id, "⚠️ Iltimos, pastdagi fanlardan birini tanlang!", replyMarkup: SubjectKeyboard.GetKeyboardForGrade(sel.Grade), cancellationToken: cancellationToken);
                }
                break;

            case UserStateStep.SelectingDifficulty:
                if (_difficultyHandler.TryParseDifficulty(text, out var diff))
                {
                    _sessionManager.SetUserDifficultySelection(telegramId, diff);
                    await _testHandler.StartTestAsync(botClient, message, user, 10, cancellationToken);
                }
                else
                {
                    await botClient.SendMessage(message.Chat.Id, "⚠️ Iltimos, murakkablik darajasini tanlang!", replyMarkup: DifficultyKeyboard.GetKeyboard(), cancellationToken: cancellationToken);
                }
                break;

            case UserStateStep.InTest:
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "⏱️ *Siz hozir test topshiryapsiz!*\n\nIltimos, javob berish uchun yuqoridagi savol ostidagi inline tugmalardan birini tanlang.",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    cancellationToken: cancellationToken);
                break;

            default:
                await _startHandler.HandleAsync(botClient, message, cancellationToken);
                break;
        }
    }
}
