using Telegram.Bot.Types.ReplyMarkups;

namespace StudentAssistant.Bot.Keyboards;

public static class SubjectKeyboard
{
    public static ReplyKeyboardMarkup GetKeyboardForGrade(int gradeNumber)
    {
        if (gradeNumber <= 4)
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "📚 Ona tili", "📐 Matematika" },
                new KeyboardButton[] { "🇬🇧 Ingliz tili", "🇷🇺 Rus tili" },
                new KeyboardButton[] { "🌿 Tabiiy fan", "🎨 Tasviriy san'at" },
                new KeyboardButton[] { "🎵 Musiqa", "🛠️ Texnologiya" },
                new KeyboardButton[] { "🕊️ Tarbiya", "⬅️ Orqaga" }
            })
            { ResizeKeyboard = true };
        }
        else if (gradeNumber <= 6)
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "📚 Ona tili va Adabiyot", "📐 Matematika" },
                new KeyboardButton[] { "📜 Tarix", "🌍 Geografiya" },
                new KeyboardButton[] { "🌿 Botanika", "💻 Informatika" },
                new KeyboardButton[] { "🇬🇧 Ingliz tili", "🇷🇺 Rus tili" },
                new KeyboardButton[] { "🎨 Tasviriy san'at", "🎵 Musiqa" },
                new KeyboardButton[] { "🛠️ Texnologiya", "🕊️ Tarbiya" },
                new KeyboardButton[] { "⬅️ Orqaga" }
            })
            { ResizeKeyboard = true };
        }
        else if (gradeNumber <= 9)
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "📚 Ona tili va Adabiyot", "📐 Algebra" },
                new KeyboardButton[] { "📏 Geometriya", "📜 Tarix" },
                new KeyboardButton[] { "🌍 Geografiya", "⚡ Fizika" },
                new KeyboardButton[] { "🧪 Kimyo", "🌿 Biologiya" },
                new KeyboardButton[] { "💻 Informatika", "🇬🇧 Ingliz tili" },
                new KeyboardButton[] { "🇷🇺 Rus tili", "⚖️ Huquq" },
                new KeyboardButton[] { "🕊️ Tarbiya", "⬅️ Orqaga" }
            })
            { ResizeKeyboard = true };
        }
        else // 10-11 sinf
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "📚 Ona tili va Adabiyot", "📐 Algebra" },
                new KeyboardButton[] { "📏 Geometriya", "📜 O'zbekiston Tarixi" },
                new KeyboardButton[] { "🌐 Jahon Tarixi", "🌍 Geografiya" },
                new KeyboardButton[] { "⚡ Fizika", "🧪 Kimyo" },
                new KeyboardButton[] { "🌿 Biologiya", "💻 Informatika" },
                new KeyboardButton[] { "🇬🇧 Ingliz tili", "🇷🇺 Rus tili" },
                new KeyboardButton[] { "⚖️ Huquq", "⬅️ Orqaga" }
            })
            { ResizeKeyboard = true };
        }
    }
}
