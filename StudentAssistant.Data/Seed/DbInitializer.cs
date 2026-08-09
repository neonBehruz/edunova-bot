using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentAssistant.Data.Context;
using StudentAssistant.Domain.Entities;
using StudentAssistant.Domain.Enums;

namespace StudentAssistant.Data.Seed;

public static class DbInitializer
{
    public static async Task InitializeAndSeedAsync(AppDbContext dbContext, ILogger logger)
    {
        logger.LogInformation("Ensuring database is created...");
        await dbContext.Database.EnsureCreatedAsync();

        logger.LogInformation("Checking and seeding subjects & questions...");

        var existingSubjects = await dbContext.Subjects.IgnoreQueryFilters().ToDictionaryAsync(s => s.Id);

        var subjectsToSeed = new List<Subject>
        {
            new Subject { Id = 1, Name = "Ingliz tili", Code = "ENG", Description = "English Grammar, Vocabulary & CEFR", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 2, Name = "Tarix", Code = "HIST", Description = "O'zbekiston va Jahon Tarixi", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 3, Name = "Matematika", Code = "MATH", Description = "Algebra va Geometriya", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 4, Name = "O'zbek tili va Adabiyot", Code = "UZB", Description = "Ona tili va Adabiyot", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 5, Name = "Fizika", Code = "PHYS", Description = "Mexanika, Elektr va Optika", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 6, Name = "Kimyo", Code = "CHEM", Description = "Anorganik va Organik kimyo", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 7, Name = "Biologiya", Code = "BIOL", Description = "Botanika, Zoologiya va Tabiiy fanlar", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 8, Name = "Informatika", Code = "INF", Description = "Dasturlash va Kompyuter savodxonligi", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 9, Name = "Rus tili", Code = "RUS", Description = "Rus tili grammatikasi va adabiyoti", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 10, Name = "Geografiya", Code = "GEO", Description = "Geografiya va Tabiat", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 11, Name = "Tasviriy san'at", Code = "ART", Description = "Tasviriy san'at va Rasm", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 12, Name = "Musiqa", Code = "MUS", Description = "Musiqa madaniyati va San'at", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 13, Name = "Texnologiya", Code = "TECH", Description = "Texnologiya va Mehnat ta'limi", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 14, Name = "Tarbiya", Code = "ETH", Description = "Tarbiya va Odob-axloq", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 15, Name = "Huquq", Code = "LAW", Description = "Konstitutsiya va Huquq asoslari", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        };

        foreach (var sub in subjectsToSeed)
        {
            if (!existingSubjects.ContainsKey(sub.Id))
            {
                dbContext.Subjects.Add(sub);
            }
            else
            {
                existingSubjects[sub.Id].Name = sub.Name;
                existingSubjects[sub.Id].Code = sub.Code;
                existingSubjects[sub.Id].Description = sub.Description;
            }
        }
        await dbContext.SaveChangesAsync();

        int geoQuestionCount = await dbContext.Questions.IgnoreQueryFilters().CountAsync(q => q.SubjectId == 10);
        if (geoQuestionCount == 0)
        {
            logger.LogInformation("Populating extended multi-subject question bank...");
            await SeedQuestionsAsync(dbContext);
        }

        logger.LogInformation("Database initialization and seeding complete!");
    }

    private static async Task SeedQuestionsAsync(AppDbContext dbContext)
    {
        long qId = (await dbContext.Questions.IgnoreQueryFilters().MaxAsync(q => (long?)q.Id) ?? 0) + 1;
        long optId = (await dbContext.AnswerOptions.IgnoreQueryFilters().MaxAsync(o => (long?)o.Id) ?? 0) + 1;

        void AddQ(long subId, CefrLevel level, DifficultyLevel diff, string text, string explanation, string correctOpt, params string[] wrongOpts)
        {
            var question = new Question
            {
                Id = qId,
                SubjectId = subId,
                Level = level,
                Difficulty = diff,
                Text = text,
                Explanation = explanation,
                Type = QuestionType.SingleChoice,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };
            dbContext.Questions.Add(question);

            var allOpts = new List<(string text, bool isCorrect)> { (correctOpt, true) };
            foreach (var w in wrongOpts)
            {
                allOpts.Add((w, false));
            }

            int order = 1;
            foreach (var (optText, isCorrect) in allOpts)
            {
                dbContext.AnswerOptions.Add(new AnswerOption
                {
                    Id = optId++,
                    QuestionId = qId,
                    Text = optText,
                    IsCorrect = isCorrect,
                    Order = order++,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });
            }
            qId++;
        }

        // --- INGLIZ TILI (Subject 1) ---
        AddQ(1, CefrLevel.A1, DifficultyLevel.Easy, "She _____ a student at the university.", "Use 'is' for third person singular (she).", "is", "are", "am", "be");
        AddQ(1, CefrLevel.A1, DifficultyLevel.Easy, "They _____ playing football in the garden.", "Use 'are' with 'they'.", "are", "is", "am", "was");

        // --- TARIX (Subject 2) ---
        AddQ(2, CefrLevel.A1, DifficultyLevel.Easy, "Alisher Navoiy qaysi yili va qayerda tug'ilgan?", "Alisher Navoiy 1441-yil 9-fevralda Hirot shahrida tavallud topgan.", "1441-yil 9-fevral, Hirot", "1336-yil 9-aprel, Kesh", "1483-yil 14-fevral, Andijon", "1207-yil 30-sentabr, Balx");
        AddQ(2, CefrLevel.A1, DifficultyLevel.Easy, "Amir Temur qachon tavallud topgan?", "Sohibqiron Amir Temur 1336-yil 9-aprelda Kesh (Shahrisabz) yaqinidagi Xoja Ilg'ar qishlog'ida tug'ilgan.", "1336-yil 9-aprel", "1441-yil 9-fevral", "1370-yil 10-may", "1405-yil 18-fevral");

        // --- MATEMATIKA (Subject 3) ---
        AddQ(3, CefrLevel.A1, DifficultyLevel.Easy, "Pifagor teoremasi qaysi uchburchak uchun o'rinli?", "a² + b² = c² teoremasi to'g'ri burchakli uchburchak uchun amal qiladi.", "To'g'ri burchakli uchburchak", "Teng tomonli uchburchak", "O'tkir burchakli uchburchak", "Teng yonli uchburchak");
        AddQ(3, CefrLevel.A1, DifficultyLevel.Easy, "2 ning 10-darajasi (2^10) nechaga teng?", "2^10 = 1024.", "1024", "512", "2048", "1000");

        // --- O'ZBEK TILI VA ADABIYOT (Subject 4) ---
        AddQ(4, CefrLevel.A1, DifficultyLevel.Easy, "'Hamsa' asari nechta dostonni o'z ichiga oladi?", "Alisher Navoiyning 'Hamsa'si 5 ta dostonni o'z ichiga oladi.", "5 ta", "4 ta", "7 ta", "3 ta");

        // --- FIZIKA (Subject 5) ---
        AddQ(5, CefrLevel.A1, DifficultyLevel.Easy, "Nyutonning ikkinchi qonuni formulasi qanday?", "F = m * a (Kuch = massa * tezlanish).", "F = m * a", "E = m * c²", "F = m * g * h", "v = s / t");

        // --- KIMYO (Subject 6) ---
        AddQ(6, CefrLevel.A1, DifficultyLevel.Easy, "Suvning kimyoviy formulasi qanday?", "Suv H₂O formulasi bilan belgilanadi.", "H₂O", "CO₂", "NaCl", "H₂SO₄");

        // --- BIOLOGIYA / TABIIY FAN (Subject 7) ---
        AddQ(7, CefrLevel.A1, DifficultyLevel.Easy, "Fotosintez jarayoni uchun nima zarur?", "Fotosintez uchun quyosh nuri, suv va karbonat angidrid zarur.", "Quyosh nuri va suv", "Faqat kislorod", "Karbamid", "Tuz");

        // --- INFORMATIKA (Subject 8) ---
        AddQ(8, CefrLevel.A1, DifficultyLevel.Easy, "1 Bayt nechta Bitga teng?", "1 Bayt = 8 Bit.", "8 bit", "10 bit", "1024 bit", "16 bit");

        // --- RUS TILI (Subject 9) ---
        AddQ(9, CefrLevel.A1, DifficultyLevel.Easy, "Как переводится слово 'Книга' на узбекский язык?", "Книга - Kitob.", "Kitob", "Daftar", "Qalam", "Maktab");

        // --- GEOGRAFIYA (Subject 10) ---
        AddQ(10, CefrLevel.A1, DifficultyLevel.Easy, "O'zbekiston Respublikasining poytaxti qaysi shahar?", "Toshkent - O'zbekiston Respublikasi poytaxti.", "Toshkent", "Samarqand", "Buxoro", "Namangan");

        // --- HUQUQ (Subject 15) ---
        AddQ(15, CefrLevel.A1, DifficultyLevel.Easy, "O'zbekiston Respublikasi Konstitutsiyasi qabul qilingan kun?", " Konstitutsiya 1992-yil 8-dekabrda qabul qilingan.", "8-dekabr", "1-sentabr", "21-mart", "14-yanvar");

        await dbContext.SaveChangesAsync();
    }
}
