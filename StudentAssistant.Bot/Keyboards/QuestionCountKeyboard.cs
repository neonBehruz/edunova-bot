using Telegram.Bot.Types.ReplyMarkups;

namespace StudentAssistant.Bot.Keyboards;

public static class QuestionCountKeyboard
{
    public static ReplyKeyboardMarkup GetKeyboard()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "5 ta savol", "10 ta savol", "20 ta savol" },
            new KeyboardButton[] { "⬅️ Orqaga" }
        })
        {
            ResizeKeyboard = true
        };
    }
}
