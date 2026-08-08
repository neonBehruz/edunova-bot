using StudentAssistant.Domain.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace StudentAssistant.Bot.Keyboards;

public static class AnswerKeyboard
{
    public static InlineKeyboardMarkup GetKeyboard(long attemptId, long questionId, List<AnswerOption> options)
    {
        var buttons = new List<InlineKeyboardButton[]>();
        char optionLetter = 'A';

        foreach (var option in options.OrderBy(o => o.Order))
        {
            string label = $"{optionLetter}) {option.Text}";
            string callbackData = $"ans_{attemptId}_{questionId}_{option.Id}";
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData(label, callbackData) });
            optionLetter++;
        }

        // Add Finish Test button
        buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("🛑 Testni yakunlash", $"finish_{attemptId}") });

        return new InlineKeyboardMarkup(buttons);
    }

    public static InlineKeyboardMarkup GetAnsweredKeyboard(List<AnswerOption> options, long selectedOptionId)
    {
        var buttons = new List<InlineKeyboardButton[]>();
        char optionLetter = 'A';

        foreach (var option in options.OrderBy(o => o.Order))
        {
            string prefix = option.Id == selectedOptionId ? "✅ " : "";
            string label = $"{prefix}{optionLetter}) {option.Text}";
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData(label, "answered_noop") });
            optionLetter++;
        }

        return new InlineKeyboardMarkup(buttons);
    }
}
