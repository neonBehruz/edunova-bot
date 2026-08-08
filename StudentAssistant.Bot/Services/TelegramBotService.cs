using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudentAssistant.Bot.Handlers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace StudentAssistant.Bot.Services;

public class TelegramBotService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly MainMenuHandler _mainMenuHandler;
    private readonly AnswerHandler _answerHandler;
    private readonly ILogger<TelegramBotService> _logger;

    public TelegramBotService(
        ITelegramBotClient botClient,
        MainMenuHandler mainMenuHandler,
        AnswerHandler answerHandler,
        ILogger<TelegramBotService> logger)
    {
        _botClient = botClient;
        _mainMenuHandler = mainMenuHandler;
        _answerHandler = answerHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
        };

        _logger.LogInformation("Telegram Bot polling starting...");

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (update.Message is { } message)
            {
                await _mainMenuHandler.HandleMessageAsync(botClient, message, cancellationToken);
            }
            else if (update.CallbackQuery is { } callbackQuery)
            {
                await _answerHandler.HandleCallbackAsync(botClient, callbackQuery, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling Telegram update");
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Telegram Bot API Error from source: {Source}", source);
        return Task.CompletedTask;
    }
}
