using Microsoft.EntityFrameworkCore;
using StudentAssistant.Domain.Entities;
using StudentAssistant.Domain.Enums;

namespace StudentAssistant.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();
    public DbSet<TestAttempt> TestAttempts => Set<TestAttempt>();
    public DbSet<StudentAnswer> StudentAnswers => Set<StudentAnswer>();
    public DbSet<QuestionHistory> QuestionHistories => Set<QuestionHistory>();
    public DbSet<UserProgress> UserProgresses => Set<UserProgress>();
    public DbSet<UserRating> UserRatings => Set<UserRating>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global Query Filter for Soft Delete
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Subject>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Question>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<AnswerOption>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<TestAttempt>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<StudentAnswer>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<QuestionHistory>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<UserProgress>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<UserRating>().HasQueryFilter(e => !e.IsDeleted);

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // 1. Seed Subjects
        var subjects = new List<Subject>
        {
            new Subject { Id = 1, Name = "Ingliz tili", Code = "ENG", Description = "English Grammar, Vocabulary & CEFR", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 2, Name = "Tarix", Code = "HIST", Description = "O'zbekiston va Jahon Tarixi", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 3, Name = "Matematika", Code = "MATH", Description = "Algebra va Geometriya", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 4, Name = "O'zbek tili va Adabiyot", Code = "UZB", Description = "O'zbek tili va Mumtoz adabiyot", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 5, Name = "Fizika", Code = "PHYS", Description = "Mexanika, Elektr va Optika", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 6, Name = "Kimyo", Code = "CHEM", Description = "Anorganik va Organik kimyo", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 7, Name = "Biologiya", Code = "BIOL", Description = "Botanika, Zoologiya va Anatomiya", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Subject { Id = 8, Name = "Informatika", Code = "INF", Description = "Dasturlash va Kompyuter savodxonligi", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        };
        modelBuilder.Entity<Subject>().HasData(subjects);

        // Seed questions & answer options across CEFR levels (A1 to C2) and difficulties
        var questions = new List<Question>();
        var options = new List<AnswerOption>();
        long qId = 1;
        long optId = 1;

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
            questions.Add(question);

            var allOpts = new List<(string text, bool isCorrect)> { (correctOpt, true) };
            foreach (var w in wrongOpts)
            {
                allOpts.Add((w, false));
            }

            int order = 1;
            foreach (var (optText, isCorrect) in allOpts)
            {
                options.Add(new AnswerOption
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
        AddQ(1, CefrLevel.A1, DifficultyLevel.Middle, "Where _____ your brother live?", "Use auxiliary 'does' for third-person singular questions.", "does", "do", "is", "are");
        AddQ(1, CefrLevel.A1, DifficultyLevel.Hard, "My sister is older _____ me.", "Comparative adjective takes 'than'.", "than", "then", "that", "as");

        // --- TARIX (Subject 2) ---
        AddQ(2, CefrLevel.A1, DifficultyLevel.Easy, "Alisher Navoiy qaysi yili va qayerda tug'ilgan?", "Alisher Navoiy 1441-yil 9-fevralda Hirot shahrida tavallud topgan.", "1441-yil 9-fevral, Hirot", "1336-yil 9-aprel, Kesh", "1483-yil 14-fevral, Andijon", "1207-yil 30-sentabr, Balx");
        AddQ(2, CefrLevel.A1, DifficultyLevel.Easy, "Amir Temur qachon tavallud topgan?", "Sohibqiron Amir Temur 1336-yil 9-aprelda Kesh (Shahrisabz) yaqinidagi Xoja Ilg'ar qishlog'ida tug'ilgan.", "1336-yil 9-aprel", "1441-yil 9-fevral", "1370-yil 10-may", "1405-yil 18-fevral");
        AddQ(2, CefrLevel.A1, DifficultyLevel.Easy, "O'zbekiston Respublikasi Mustaqilligi qaysi kuni e'lon qilingan?", "1991-yil 31-avgustda O'zbekiston Respublikasining Davlat mustaqilligi e'lon qilingan.", "1991-yil 31-avgust", "1990-yil 20-iyun", "1992-yil 8-dekabr", "1994-yil 1-iyul");
        AddQ(2, CefrLevel.A1, DifficultyLevel.Middle, "Zahiriddin Muhammad Bobur qaysi sulola va imperiyaga asos solgan?", "Bobur Hindistonda Boburiylar (Buyuk Mo'g'ullar) imperiyasiga asos solgan.", "Boburiylar (Buyuk Mo'g'ullar)", "Temuriylar", "G'aznaviylar", "Osmoniylar");
        AddQ(2, CefrLevel.A1, DifficultyLevel.Hard, "Mirzo Ulug'bek rasadxonasi qaysi shaharda barpo etilgan?", "Ulug'bek rasadxonasi XV asrda Samarqand shahrida qurilgan.", "Samarqand", "Buxoro", "Xiva", "Toshkent");
        AddQ(2, CefrLevel.A2, DifficultyLevel.Easy, "Jaloliddin Manguberdi qaysi davlat hukmdori va milliy qahramoni bo'lgan?", "Jaloliddin Manguberdi Xorazmshohlar davlati hukmdori bo'lgan.", "Xorazmshohlar", "Qoraxoniylar", "Somoniylar", "G'aznaviylar");
        AddQ(2, CefrLevel.A2, DifficultyLevel.Middle, "Somoniylar davlatining poytaxti qaysi shahar bo'lgan?", "Somoniylar poytaxti Buxoro shahri bo'lgan.", "Buxoro", "Samarqand", "Toshkent", "Urganch");

        // --- MATEMATIKA (Subject 3) ---
        AddQ(3, CefrLevel.A1, DifficultyLevel.Easy, "Pifagor teoremasi qaysi uchburchak uchun o'rinli?", "a² + b² = c² teoremasi to'g'ri burchakli uchburchak uchun amal qiladi.", "To'g'ri burchakli uchburchak", "Teng tomonli uchburchak", "O'tkir burchakli uchburchak", "Teng yonli uchburchak");
        AddQ(3, CefrLevel.A1, DifficultyLevel.Easy, "2 ning 10-darajasi (2^10) nechaga teng?", "2^10 = 1024.", "1024", "512", "2048", "1000");
        AddQ(3, CefrLevel.A1, DifficultyLevel.Middle, "Quyidagilardan qaysi biri tub son?", "17 faqat 1 ga va o'ziga bo'linadi.", "17", "15", "21", "27");
        AddQ(3, CefrLevel.A1, DifficultyLevel.Hard, "Kvadratning yuzi 64 sm² bo'lsa, uning perimetri nechaga teng?", "Tomoni a = √64 = 8 sm, perimetri P = 4 * 8 = 32 sm.", "32 sm", "16 sm", "24 sm", "64 sm");
        AddQ(3, CefrLevel.A2, DifficultyLevel.Easy, "Uchburchakning ichki burchaklari yig'indisi nechaga teng?", "Barcha uchburchaklarda ichki burchaklar yig'indisi 180°.", "180°", "360°", "90°", "270°");

        // --- O'ZBEK TILI VA ADABIYOT (Subject 4) ---
        AddQ(4, CefrLevel.A1, DifficultyLevel.Easy, "'Hamsa' asari nechta dostonni o'z ichiga oladi?", "Alisher Navoiyning 'Hamsa'si 5 ta dostonni o'z ichiga oladi.", "5 ta", "4 ta", "7 ta", "3 ta");
        AddQ(4, CefrLevel.A1, DifficultyLevel.Easy, "'O'tkan kunlar' birinchi o'zbek romani muallifi kim?", "Abdulla Qodiriy birinchi o'zbek romani 'O'tkan kunlar'ni yozgan.", "Abdulla Qodiriy", "Cho'lpon", "Oybek", "Said Ahmad");
        AddQ(4, CefrLevel.A1, DifficultyLevel.Middle, "O'zbek tilida nechta unli tovush bor?", "Hozirgi o'zbek adabiy tilida 6 ta unli tovush bor (a, o, i, u, o', e).", "6 ta", "5 ta", "10 ta", "8 ta");
        AddQ(4, CefrLevel.A2, DifficultyLevel.Easy, "'Kecha va kunduz' romani muallifi kim?", "Abdulhamid Sulaymon o'g'li Cho'lpon yozgan.", "Cho'lpon", "Abdulla Qodiriy", "Fitrat", "Oybek");

        // --- FIZIKA (Subject 5) ---
        AddQ(5, CefrLevel.A1, DifficultyLevel.Easy, "Nyutonning ikkinchi qonuni formulasi qanday?", "F = m * a (Kuch = massa * tezlanish).", "F = m * a", "E = m * c²", "F = m * g * h", "v = s / t");
        AddQ(5, CefrLevel.A1, DifficultyLevel.Middle, "Yorug'likning vakuumdagi tezligi nechaga teng?", "Yorug'lik tezligi c ≈ 300 000 km/s.", "300 000 km/s", "150 000 km/s", "3000 km/s", "1 000 000 km/s");

        // --- KIMYO (Subject 6) ---
        AddQ(6, CefrLevel.A1, DifficultyLevel.Easy, "Suvning kimyoviy formulasi qanday?", "Suv H₂O formulasi bilan belgilanadi.", "H₂O", "CO₂", "NaCl", "H₂SO₄");
        AddQ(6, CefrLevel.A1, DifficultyLevel.Middle, "Osh tuzining kimyoviy formulasi qanday?", "Natriy xlorid: NaCl.", "NaCl", "KCl", "NaOH", "HCl");

        // --- BIOLOGIYA (Subject 7) ---
        AddQ(7, CefrLevel.A1, DifficultyLevel.Easy, "Odam organizmidagi eng katta a'zo qaysi?", "Inson organizmidagi eng katta a'zo teri hisoblanadi.", "Teri", "Jigar", "Yurak", "O'pka");
        AddQ(7, CefrLevel.A1, DifficultyLevel.Middle, "Fotosintez jarayoni o'simlikning qaysi organoidida kechadi?", "Fotosintez xloroplastlarda kechadi.", "Xloroplast", "Mitoxondriya", "Yadro", "Ribosoma");

        // --- INFORMATIKA (Subject 8) ---
        AddQ(8, CefrLevel.A1, DifficultyLevel.Easy, "1 Bayt nechta Bitga teng?", "1 Bayt = 8 Bit.", "8 bit", "10 bit", "1024 bit", "16 bit");
        AddQ(8, CefrLevel.A1, DifficultyLevel.Middle, "Python dasturlash tilida ekranga matn chiqarish funksiyasi qaysi?", "Python'da print() funksiyasi ishlatiladi.", "print()", "cout <<", "Console.WriteLine()", "printf()");

        modelBuilder.Entity<Question>().HasData(questions);
        modelBuilder.Entity<AnswerOption>().HasData(options);
    }
}
