using StudentAssistant.Bot.Keyboards;
using StudentAssistant.Service.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StudentAssistant.Bot.Handlers;

public class RatingHandler
{
    private readonly IRatingService _ratingService;

    public RatingHandler(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    public async Task SendLeaderboardAsync(ITelegramBotClient botClient, Message message, long userId, CancellationToken cancellationToken)
    {
        var topRatings = await _ratingService.GetTopRatingsAsync(10);
        var currentUserRating = await _ratingService.GetUserRatingAsync(userId);

        string text = "🏆 *Reyting*\n\n";

        foreach (var r in topRatings)
        {
            string medal = r.Rank == 1 ? "🥇" : r.Rank == 2 ? "🥈" : r.Rank == 3 ? "🥉" : $"{r.Rank} ";
            text += $"{medal} *{r.FirstName}*\t\t\t\t*{r.RatingScore} XP*\n";
        }

        if (currentUserRating != null)
        {
            text += $"\n-----------------------------------\n" +
                    $"🎯 *Sizning o'rningiz:* *#{currentUserRating.Rank}* ({currentUserRating.RatingScore} XP)";
        }

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: text,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: MainMenuKeyboard.GetKeyboard(),
            cancellationToken: cancellationToken);
    }
}
