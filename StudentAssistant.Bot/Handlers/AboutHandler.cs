using StudentAssistant.Bot.Keyboards;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StudentAssistant.Bot.Handlers;

public class AboutHandler
{
    public async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        string text = "ℹ️ *Bot haqida*\n\n" +
                      "🎓 *EduNova* — bilimni sinash va rivojlantirish uchun aqlli test bot.\n\n" +
                      "✅ A1 – C2 darajalar\n" +
                      "✅ Easy / Middle / Hard\n" +
                      "✅ Vaqtli testlar\n" +
                      "✅ Takrorlanmaydigan savollar\n" +
                      "✅ Natijalar va progress\n" +
                      "✅ Reyting tizimi\n\n" +
                      "Learn. Test. Improve. 🚀";

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: text,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: MainMenuKeyboard.GetKeyboard(),
            cancellationToken: cancellationToken);
    }
}

public class SupportHandler
{
    public async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        string text = "🆘 *Support*\n\n" +
                      "Bot bilan bog'liq muammo yoki taklifingiz bormi?\n\n" +
                      "📩 *Administrator bilan bog'laning:*\n\n" +
                      "👤 @Sagdullayev\\_Behruz";

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[] { InlineKeyboardButton.WithUrl("✉️ Xabar yozish", "https://t.me/Sagdullayev_Behruz") }
        });

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: text,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }
}
