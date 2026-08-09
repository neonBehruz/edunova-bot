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
            new Subject { Id = 1, Name = "Ingliz tili", Code = "ENG", Description = "English Grammar & Vocabulary", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 2, Name = "Tarix", Code = "HIST", Description = "O'zbekiston va Jahon Tarixi", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 3, Name = "Matematika", Code = "MATH", Description = "Algebra va Geometriya", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 4, Name = "Ona tili va Adabiyot", Code = "UZB", Description = "Ona tili va Adabiyot", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 5, Name = "Fizika", Code = "PHYS", Description = "Fizika", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 6, Name = "Kimyo", Code = "CHEM", Description = "Anorganik va Organik kimyo", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 7, Name = "Biologiya", Code = "BIOL", Description = "Botanika, Biologiya va Tabiiy fanlar", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 8, Name = "Informatika", Code = "INF", Description = "Dasturlash va Kompyuter savodxonligi", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 9, Name = "Rus tili", Code = "RUS", Description = "Rus tili", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 10, Name = "Geografiya", Code = "GEO", Description = "Geografiya", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 11, Name = "Tasviriy san'at", Code = "ART", Description = "Tasviriy san'at va Rasm", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 12, Name = "Musiqa", Code = "MUS", Description = "Musiqa madaniyati", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 13, Name = "Texnologiya", Code = "TECH", Description = "Texnologiya", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 14, Name = "Tarbiya", Code = "ETH", Description = "Tarbiya", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 15, Name = "Huquq", Code = "LAW", Description = "Davlat va Huquq asoslari", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 16, Name = "Alifbe", Code = "ALF", Description = "Alifbe", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 17, Name = "Yozuv", Code = "WRT", Description = "Yozuv", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 18, Name = "O'qish savodxonligi", Code = "READ", Description = "O'qish savodxonligi", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 19, Name = "Astronomiya", Code = "AST", Description = "Astronomiya", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 20, Name = "Iqtisodiy bilim asoslari", Code = "ECO", Description = "Iqtisodiy bilim", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 21, Name = "Tadbirkorlik asoslari", Code = "ENT", Description = "Tadbirkorlik", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 22, Name = "Chizmachilik", Code = "DRW", Description = "Chizmachilik", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
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

        int totalQuestionCount = await dbContext.Questions.IgnoreQueryFilters().CountAsync();
        if (totalQuestionCount < 50)
        {
            logger.LogInformation("Populating comprehensive multi-subject question bank...");
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
        AddQ(1, CefrLevel.A1, DifficultyLevel.Easy, "I _____ to school every day.", "Present simple with 'I' takes base verb 'go'.", "go", "goes", "going", "gone");
        AddQ(1, CefrLevel.A1, DifficultyLevel.Easy, "What _____ your name?", "Use 'is' for singular noun 'name'.", "is", "are", "am", "be");

        // --- TARIX (Subject 2) ---
        AddQ(2, CefrLevel.A1, DifficultyLevel.Easy, "Alisher Navoiy qaysi yili va qayerda tug'ilgan?", "Alisher Navoiy 1441-yil 9-fevralda Hirot shahrida tavallud topgan.", "1441-yil 9-fevral, Hirot", "1336-yil 9-aprel, Kesh", "1483-yil 14-fevral, Andijon", "1207-yil 30-sentabr, Balx");
        AddQ(2, CefrLevel.A1, DifficultyLevel.Easy, "Amir Temur qachon tavallud topgan?", "Sohibqiron Amir Temur 1336-yil 9-aprelda Keshda tug'ilgan.", "1336-yil 9-aprel", "1441-yil 9-fevral", "1370-yil 10-may", "1405-yil 18-fevral");
        AddQ(2, CefrLevel.A1, DifficultyLevel.Easy, "O'zbekiston Respublikasi Mustaqilligi qaysi kuni e'lon qilingan?", "1991-yil 31-avgustda e'lon qilingan.", "1991-yil 31-avgust", "1990-yil 20-iyun", "1992-yil 8-dekabr", "1994-yil 1-iyul");

        // --- MATEMATIKA (Subject 3) ---
        AddQ(3, CefrLevel.A1, DifficultyLevel.Easy, "5 + 7 yig'indi nechaga teng?", "5 va 7 ning yig'indisi 12 bo'ladi.", "12", "11", "13", "10");
        AddQ(3, CefrLevel.A1, DifficultyLevel.Easy, "8 * 9 ko'paytma nechaga teng?", "8 * 9 = 72.", "72", "64", "81", "70");
        AddQ(3, CefrLevel.A1, DifficultyLevel.Easy, "Pifagor teoremasi qaysi uchburchak uchun o'rinli?", "To'g'ri burchakli uchburchak uchun.", "To'g'ri burchakli uchburchak", "Teng tomonli uchburchak", "O'tkir burchakli uchburchak", "Teng yonli uchburchak");

        // --- ONA TILI VA ADABIYOT (Subject 4) ---
        AddQ(4, CefrLevel.A1, DifficultyLevel.Easy, "'Hamsa' asari nechta dostonni o'z ichiga oladi?", "Alisher Navoiyning 'Hamsa'si 5 ta dostonni o'z ichiga oladi.", "5 ta", "4 ta", "7 ta", "3 ta");
        AddQ(4, CefrLevel.A1, DifficultyLevel.Easy, "O'zbek tilida nechta unli tovush bor?", "Hozirgi o'zbek adabiy tilida 6 ta unli tovush bor.", "6 ta", "5 ta", "10 ta", "8 ta");
        AddQ(4, CefrLevel.A1, DifficultyLevel.Easy, "'O'tkan kunlar' romanining muallifi kim?", "Abdulla Qodiriy yozgan.", "Abdulla Qodiriy", "Cho'lpon", "Oybek", "Said Ahmad");

        // --- FIZIKA (Subject 5) ---
        AddQ(5, CefrLevel.A1, DifficultyLevel.Easy, "Nyutonning ikkinchi qonuni formulasi qanday?", "F = m * a.", "F = m * a", "E = m * c²", "F = m * g * h", "v = s / t");
        AddQ(5, CefrLevel.A1, DifficultyLevel.Easy, "Tezlik birligi xalqaro birliklar tizimida nima?", "Tezlik birligi m/s (metr taqsim sekund).", "m/s", "km", "kg", "N");

        // --- KIMYO (Subject 6) ---
        AddQ(6, CefrLevel.A1, DifficultyLevel.Easy, "Suvning kimyoviy formulasi qanday?", "Suv H₂O formulasi bilan belgilanadi.", "H₂O", "CO₂", "NaCl", "H₂SO₄");
        AddQ(6, CefrLevel.A1, DifficultyLevel.Easy, "Osh tuzining kimyoviy formulasi qanday?", "Natriy xlorid: NaCl.", "NaCl", "KCl", "NaOH", "HCl");

        // --- BIOLOGIYA / TABIIY FAN (Subject 7) ---
        AddQ(7, CefrLevel.A1, DifficultyLevel.Easy, "Fotosintez jarayoni o'simlikning qaysi qismida kechadi?", "Fotosintez xloroplastlarda kechadi.", "Xloroplast", "Yadro", "Mitoxondriya", "Ribosoma");
        AddQ(7, CefrLevel.A1, DifficultyLevel.Easy, "Odam organizmidagi eng katta a'zo qaysi?", "Eng katta a'zo teri hisoblanadi.", "Teri", "Jigar", "Yurak", "O'pka");

        // --- INFORMATIKA (Subject 8) ---
        AddQ(8, CefrLevel.A1, DifficultyLevel.Easy, "1 Bayt nechta Bitga teng?", "1 Bayt = 8 Bit.", "8 bit", "10 bit", "1024 bit", "16 bit");
        AddQ(8, CefrLevel.A1, DifficultyLevel.Easy, "Python dasturlash tilida ekranga matn chiqarish funksiyasi qaysi?", "print() funksiyasi ishlatiladi.", "print()", "cout <<", "Console.WriteLine()", "printf()");

        // --- RUS TILI (Subject 9) ---
        AddQ(9, CefrLevel.A1, DifficultyLevel.Easy, "Как переводится слово 'Книга' на узбекский язык?", "Книга - Kitob.", "Kitob", "Daftar", "Qalam", "Maktab");
        AddQ(9, CefrLevel.A1, DifficultyLevel.Easy, "Как переводится слово 'Школа' на узбекский язык?", "Школа - Maktab.", "Maktab", "Uy", "Shahar", "Daryo");

        // --- GEOGRAFIYA (Subject 10) ---
        AddQ(10, CefrLevel.A1, DifficultyLevel.Easy, "O'zbekiston Respublikasining poytaxti qaysi shahar?", "Toshkent - O'zbekiston Respublikasi poytaxti.", "Toshkent", "Samarqand", "Buxoro", "Namangan");
        AddQ(10, CefrLevel.A1, DifficultyLevel.Easy, "Dunyodagi eng katta okean qaysi?", "Tinch okeani eng katta okeandir.", "Tinch okeani", "Atlantika okeani", "Hind okeani", "Shimoliy Muz okeani");

        // --- TASVIRTIY SAN'AT (Subject 11) ---
        AddQ(11, CefrLevel.A1, DifficultyLevel.Easy, "Asosiy ranglar qaysilar?", "Qizil, sariq, ko'k asosiy ranglar hisoblanadi.", "Qizil, sariq, ko'k", "Yashil, binafsha, toq sariq", "Oq va qora", "Jigarrang va kulrang");

        // --- MUSIQA (Subject 12) ---
        AddQ(12, CefrLevel.A1, DifficultyLevel.Easy, "Musiqa notasida nechta asosiy nota bor?", "7 ta asosiy nota bor (Do, Re, Mi, Fa, Sol, Lya, Si).", "7 ta", "5 ta", "8 ta", "12 ta");

        // --- TEXNOLOGIYA (Subject 13) ---
        AddQ(13, CefrLevel.A1, DifficultyLevel.Easy, "Yog'ochga ishlov berishda qaysi asbob ishlatiladi?", "Randa va arsa yog'ochga ishlov beradi.", "Arra va randa", "Ombur", "Bolg'a", "Qaychi");

        // --- TARBIYA (Subject 14) ---
        AddQ(14, CefrLevel.A1, DifficultyLevel.Easy, "Kattalarga va ustozlarga qanday munosabatda bo'lish kerak?", "Kattalarga va ustozlarga hamisha hurmat ko'rsatish kerak.", "Hurmat va ehtirom ko'rsatish", "E'tiborsiz bo'lish", "Faqat salom berish", "Farqi yo'q");

        // --- HUQUQ (Subject 15) ---
        AddQ(15, CefrLevel.A1, DifficultyLevel.Easy, "O'zbekiston Respublikasi Konstitutsiyasi qaysi kuni qabul qilingan?", "Konstitutsiya 8-dekabrda qabul qilingan.", "8-dekabr", "1-sentabr", "21-mart", "14-yanvar");

        // --- ALIFBE (Subject 16) ---
        AddQ(16, CefrLevel.A1, DifficultyLevel.Easy, "O'zbek alifbosida birinchi harf qaysi?", "Alifboda birinchi harf 'A'.", "A harfi", "B harfi", "V harfi", "Z harfi");

        // --- YOZUV (Subject 17) ---
        AddQ(17, CefrLevel.A1, DifficultyLevel.Easy, "Gap nimadan boshlanadi?", "Barcha gaplar katta harf bilan boshlanadi.", "Katta harf bilan", "Kichik harf bilan", "Nuqta bilan", "Raqam bilan");

        // --- O'QISH SAVODXONLIGI (Subject 18) ---
        AddQ(18, CefrLevel.A1, DifficultyLevel.Easy, "Kitob o'qish inson uchun qanday foyda beradi?", "Kitob bilimlarni oshiradi va tafakkurni kengaytiradi.", "Bilim va dunyoqarashni kengaytiradi", "Faqat vaqt o'tkazadi", "Hormatni kamaytiradi", "Hech qanday");

        // --- ASTRONOMIYA (Subject 19) ---
        AddQ(19, CefrLevel.A1, DifficultyLevel.Easy, "Quyosh tizimidagi markaziy yulduz nima?", "Quyosh - tizimdagi markaziy yulduzdir.", "Quyosh", "Oy", "Yupiter", "Mars");

        // --- IQTISODIY BILIM (Subject 20) ---
        AddQ(20, CefrLevel.A1, DifficultyLevel.Easy, "Pulning asosiy vazifasi nima?", "Pul tovar va xizmatlarni ayriboshlash vositasidir.", "Muomala va to'lov vositasi", "Faqat bezak", "Qog'oz bo'lagi", "Farqi yo'q");

        // --- TADBIRKORLIK (Subject 21) ---
        AddQ(21, CefrLevel.A1, DifficultyLevel.Easy, "Tadbirkorlik faoliyatining asosiy maqsadi nima?", "Qonuniy yo'l bilan foyda olish va elga xizmat qilish.", "Qonuniy foyda olish", "Zarar ko'rish", "Hech narsa qilmaslik", "Faqat xarajat qilish");

        // --- CHIZMACHILIK (Subject 22) ---
        AddQ(22, CefrLevel.A1, DifficultyLevel.Easy, "Chizma chizishda asosiy asbob nima?", "Chizg'ich va qalam chizmachilikning asosiy qurollaridir.", "Chizg'ich va qalam", "Qaychi", "Bo'yoq", "Yelim");

        await dbContext.SaveChangesAsync();
    }
}
