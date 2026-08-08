using StudentAssistant.Bot.Keyboards;
using StudentAssistant.Bot.State;
using StudentAssistant.Service.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentAssistant.Bot.Handlers;

public class StartHandler
{
    private readonly IUserService _userService;
    private readonly TestSessionManager _sessionManager;

    public StartHandler(IUserService userService, TestSessionManager sessionManager)
    {
        _userService = userService;
        _sessionManager = sessionManager;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var telegramUser = message.From;
        if (telegramUser == null) return;

        var user = await _userService.GetOrCreateUserAsync(
            telegramUser.Id,
            telegramUser.FirstName,
            telegramUser.LastName,
            telegramUser.Username);

        _sessionManager.SetUserState(user.Id, UserStateStep.MainMenu);

        string welcomeText = "👋 *Assalomu alaykum!*\n" +
                             "*EduNova ga xush kelibsiz.*\n\n" +
                             "Bilimingizni sinang, darajangizni oshiring va eng yaxshilar safidan o'rin oling! 🚀";

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: welcomeText,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: MainMenuKeyboard.GetKeyboard(),
            cancellationToken: cancellationToken);
    }
}
