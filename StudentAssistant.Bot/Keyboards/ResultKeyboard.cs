using Telegram.Bot.Types.ReplyMarkups;

namespace StudentAssistant.Bot.Keyboards;

public static class ResultKeyboard
{
    public static ReplyKeyboardMarkup GetKeyboard()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "🎯 Yangi test boshlash" },
            new KeyboardButton[] { "🏠 Bosh menyu" }
        })
        {
            ResizeKeyboard = true
        };
    }
}

public static class BackKeyboard
{
    public static ReplyKeyboardMarkup GetKeyboard()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "⬅️ Orqaga" }
        })
        {
            ResizeKeyboard = true
        };
    }
}
