using Telegram.Bot.Types.ReplyMarkups;

namespace StudentAssistant.Bot.Keyboards;

public static class MainMenuKeyboard
{
    public static ReplyKeyboardMarkup GetKeyboard()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "🎯 Test boshlash" },
            new KeyboardButton[] { "📊 Natijalarim" },
            new KeyboardButton[] { "🏆 Reyting" },
            new KeyboardButton[] { "ℹ️ Bot haqida" },
            new KeyboardButton[] { "🆘 Support" }
        })
        {
            ResizeKeyboard = true,
            IsPersistent = true
        };
    }
}
