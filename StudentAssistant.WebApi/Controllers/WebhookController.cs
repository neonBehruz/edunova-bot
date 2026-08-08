using Microsoft.AspNetCore.Mvc;
using StudentAssistant.Bot.Handlers;
using Telegram.Bot.Types;

namespace StudentAssistant.WebApi.Controllers;

[ApiController]
[Route("api/webhook")]
public class WebhookController : ControllerBase
{
    private readonly MainMenuHandler _mainMenuHandler;
    private readonly AnswerHandler _answerHandler;

    public WebhookController(MainMenuHandler mainMenuHandler, AnswerHandler answerHandler)
    {
        _mainMenuHandler = mainMenuHandler;
        _answerHandler = answerHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update, [FromServices] Telegram.Bot.ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        if (update.Message is { } message)
        {
            await _mainMenuHandler.HandleMessageAsync(botClient, message, cancellationToken);
        }
        else if (update.CallbackQuery is { } callbackQuery)
        {
            await _answerHandler.HandleCallbackAsync(botClient, callbackQuery, cancellationToken);
        }

        return Ok();
    }
}
