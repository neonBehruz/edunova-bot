using Telegram.Bot.Types.ReplyMarkups;

namespace StudentAssistant.Bot.Keyboards;

public static class SubjectKeyboard
{
    public static ReplyKeyboardMarkup GetKeyboardForGrade(int gradeNumber)
    {
        var rows = new List<KeyboardButton[]>();

        switch (gradeNumber)
        {
            case 1:
                rows.Add(new KeyboardButton[] { "📚 Alifbe", "✍️ Yozuv" });
                rows.Add(new KeyboardButton[] { "📚 Ona tili", "📖 O‘qish savodxonligi" });
                rows.Add(new KeyboardButton[] { "📐 Matematika", "🌿 Tabiiy fan" });
                rows.Add(new KeyboardButton[] { "🕊️ Tarbiya", "🇬🇧 Chet tili" });
                rows.Add(new KeyboardButton[] { "💻 Informatika", "🎵 Musiqa" });
                rows.Add(new KeyboardButton[] { "🎨 Tasviriy san’at", "🛠️ Texnologiya" });
                rows.Add(new KeyboardButton[] { "⚽ Jismoniy tarbiya", "⬅️ Orqaga" });
                break;

            case 2:
            case 3:
            case 4:
                rows.Add(new KeyboardButton[] { "📚 Ona tili", "📖 O‘qish savodxonligi" });
                rows.Add(new KeyboardButton[] { "📐 Matematika", "🌿 Tabiiy fan" });
                rows.Add(new KeyboardButton[] { "🕊️ Tarbiya", "🇬🇧 Chet tili" });
                rows.Add(new KeyboardButton[] { "💻 Informatika", "🎵 Musiqa" });
                rows.Add(new KeyboardButton[] { "🎨 Tasviriy san’at", "🛠️ Texnologiya" });
                rows.Add(new KeyboardButton[] { "⚽ Jismoniy tarbiya", "⬅️ Orqaga" });
                break;

            case 5:
            case 6:
                rows.Add(new KeyboardButton[] { "📚 Ona tili", "📖 Adabiyot" });
                rows.Add(new KeyboardButton[] { "🇷🇺 Rus tili", "🇬🇧 Chet tili" });
                rows.Add(new KeyboardButton[] { "📐 Matematika", "📜 Tarix" });
                rows.Add(new KeyboardButton[] { "🕊️ Tarbiya", "💻 Informatika" });
                rows.Add(new KeyboardButton[] { "🌿 Tabiiy fan", "🎵 Musiqa" });
                rows.Add(new KeyboardButton[] { "🎨 Tasviriy san’at", "🛠️ Texnologiya" });
                rows.Add(new KeyboardButton[] { "⚽ Jismoniy tarbiya", "⬅️ Orqaga" });
                break;

            case 7:
                rows.Add(new KeyboardButton[] { "📚 Ona tili", "📖 Adabiyot" });
                rows.Add(new KeyboardButton[] { "🇷🇺 Rus tili", "🇬🇧 Chet tili" });
                rows.Add(new KeyboardButton[] { "📐 Algebra", "📏 Geometriya" });
                rows.Add(new KeyboardButton[] { "📜 O‘zbekiston tarixi", "🌐 Jahon tarixi" });
                rows.Add(new KeyboardButton[] { "⚡ Fizika", "🌿 Biologiya" });
                rows.Add(new KeyboardButton[] { "🌍 Geografiya", "💻 Informatika" });
                rows.Add(new KeyboardButton[] { "🎨 Tasviriy san’at", "🛠️ Texnologiya" });
                rows.Add(new KeyboardButton[] { "🕊️ Tarbiya", "⚽ Jismoniy tarbiya" });
                rows.Add(new KeyboardButton[] { "⬅️ Orqaga" });
                break;

            case 8:
                rows.Add(new KeyboardButton[] { "📚 Ona tili", "📖 Adabiyot" });
                rows.Add(new KeyboardButton[] { "🇷🇺 Rus tili", "🇬🇧 Chet tili" });
                rows.Add(new KeyboardButton[] { "📐 Algebra", "📏 Geometriya" });
                rows.Add(new KeyboardButton[] { "📜 O‘zbekiston tarixi", "🌐 Jahon tarixi" });
                rows.Add(new KeyboardButton[] { "⚡ Fizika", "🧪 Kimyo" });
                rows.Add(new KeyboardButton[] { "🌿 Biologiya", "🌍 Geografiya" });
                rows.Add(new KeyboardButton[] { "💻 Informatika", "💰 Iqtisodiy bilim asoslari" });
                rows.Add(new KeyboardButton[] { "📐 Chizmachilik", "🛠️ Texnologiya" });
                rows.Add(new KeyboardButton[] { "🕊️ Tarbiya", "⚽ Jismoniy tarbiya" });
                rows.Add(new KeyboardButton[] { "⬅️ Orqaga" });
                break;

            case 9:
                rows.Add(new KeyboardButton[] { "📚 Ona tili", "📖 Adabiyot" });
                rows.Add(new KeyboardButton[] { "🇷🇺 Rus tili", "🇬🇧 Chet tili" });
                rows.Add(new KeyboardButton[] { "📐 Algebra", "📏 Geometriya" });
                rows.Add(new KeyboardButton[] { "📜 O‘zbekiston tarixi", "🌐 Jahon tarixi" });
                rows.Add(new KeyboardButton[] { "⚡ Fizika", "🧪 Kimyo" });
                rows.Add(new KeyboardButton[] { "🌿 Biologiya", "🌍 Geografiya" });
                rows.Add(new KeyboardButton[] { "⚖️ Davlat va huquq asoslari", "💰 Iqtisodiy bilim asoslari" });
                rows.Add(new KeyboardButton[] { "📐 Chizmachilik", "💻 Informatika" });
                rows.Add(new KeyboardButton[] { "🕊️ Tarbiya", "⚽ Jismoniy tarbiya" });
                rows.Add(new KeyboardButton[] { "⬅️ Orqaga" });
                break;

            case 10:
                rows.Add(new KeyboardButton[] { "📚 Ona tili", "📖 Adabiyot" });
                rows.Add(new KeyboardButton[] { "🇷🇺 Rus tili", "🇬🇧 Chet tili" });
                rows.Add(new KeyboardButton[] { "📐 Algebra va matematik analiz", "📏 Geometriya" });
                rows.Add(new KeyboardButton[] { "📜 O‘zbekiston tarixi", "🌐 Jahon tarixi" });
                rows.Add(new KeyboardButton[] { "⚡ Fizika", "🧪 Kimyo" });
                rows.Add(new KeyboardButton[] { "🌿 Biologiya", "🌍 Geografiya" });
                rows.Add(new KeyboardButton[] { "⚖️ Davlat va huquq asoslari", "💼 Tadbirkorlik asoslari" });
                rows.Add(new KeyboardButton[] { "💻 Informatika", "🛠️ Texnologiya" });
                rows.Add(new KeyboardButton[] { "🕊️ Tarbiya", "⚽ Jismoniy tarbiya" });
                rows.Add(new KeyboardButton[] { "⬅️ Orqaga" });
                break;

            case 11:
            default:
                rows.Add(new KeyboardButton[] { "📚 Ona tili", "📖 Adabiyot" });
                rows.Add(new KeyboardButton[] { "🇷🇺 Rus tili", "🇬🇧 Chet tili" });
                rows.Add(new KeyboardButton[] { "📐 Algebra va matematik analiz", "📏 Geometriya" });
                rows.Add(new KeyboardButton[] { "📜 O‘zbekiston tarixi", "🌐 Jahon tarixi" });
                rows.Add(new KeyboardButton[] { "⚡ Fizika", "🪐 Astronomiya" });
                rows.Add(new KeyboardButton[] { "🧪 Kimyo", "🌿 Biologiya" });
                rows.Add(new KeyboardButton[] { "🌍 Geografiya", "⚖️ Davlat va huquq asoslari" });
                rows.Add(new KeyboardButton[] { "💼 Tadbirkorlik asoslari", "💻 Informatika" });
                rows.Add(new KeyboardButton[] { "🕊️ Tarbiya", "⚽ Jismoniy tarbiya" });
                rows.Add(new KeyboardButton[] { "⬅️ Orqaga" });
                break;
        }

        return new ReplyKeyboardMarkup(rows) { ResizeKeyboard = true };
    }
}
