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
        // 1. Seed Subject
        var englishSubject = new Subject
        {
            Id = 1,
            Name = "English Language",
            Code = "ENG",
            Description = "General English Grammar, Vocabulary and Reading Comprehension",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        modelBuilder.Entity<Subject>().HasData(englishSubject);

        // Seed questions & answer options across CEFR levels (A1 to C2) and difficulties
        var questions = new List<Question>();
        var options = new List<AnswerOption>();
        long qId = 1;
        long optId = 1;

        void AddQ(CefrLevel level, DifficultyLevel diff, string text, string explanation, string correctOpt, params string[] wrongOpts)
        {
            var question = new Question
            {
                Id = qId,
                SubjectId = 1,
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

            // Shuffle options reproducibly using fixed order
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

        // A1 Easy
        AddQ(CefrLevel.A1, DifficultyLevel.Easy, "She _____ a student at the university.", "Use 'is' for third person singular (she).", "is", "are", "am", "be");
        AddQ(CefrLevel.A1, DifficultyLevel.Easy, "They _____ playing football in the garden.", "Use 'are' with 'they'.", "are", "is", "am", "was");
        AddQ(CefrLevel.A1, DifficultyLevel.Easy, "I _____ to school every day.", "Present simple with 'I' takes the base verb 'go'.", "go", "goes", "going", "gone");
        AddQ(CefrLevel.A1, DifficultyLevel.Easy, "What is the opposite of 'big'?", "'Small' is the opposite of 'big'.", "small", "tall", "long", "fast");
        AddQ(CefrLevel.A1, DifficultyLevel.Easy, "Look at _____ dog over there!", "'that' is used for singular distant objects.", "that", "this", "these", "those");

        // A1 Middle
        AddQ(CefrLevel.A1, DifficultyLevel.Middle, "Where _____ your brother live?", "Use auxiliary 'does' for third-person singular questions.", "does", "do", "is", "are");
        AddQ(CefrLevel.A1, DifficultyLevel.Middle, "There _____ many apples in the basket.", "Use 'are' for plural countable nouns.", "are", "is", "be", "was");
        AddQ(CefrLevel.A1, DifficultyLevel.Middle, "We don't have _____ milk left.", "Use 'any' in negative sentences.", "any", "some", "many", "a");
        AddQ(CefrLevel.A1, DifficultyLevel.Middle, "He always _____ breakfast at 8 AM.", "Third-person singular present simple takes '-s' ('has' or 'eats').", "eats", "eat", "eating", "ate");
        AddQ(CefrLevel.A1, DifficultyLevel.Middle, "Can you _____ me your pencil?", "'Lend' means give temporarily.", "lend", "borrow", "take", "bring");

        // A1 Hard
        AddQ(CefrLevel.A1, DifficultyLevel.Hard, "My sister is older _____ me.", "Comparative adjective takes 'than'.", "than", "then", "that", "as");
        AddQ(CefrLevel.A1, DifficultyLevel.Hard, "He was born _____ May 15th.", "Use 'on' before specific dates.", "on", "in", "at", "by");
        AddQ(CefrLevel.A1, DifficultyLevel.Hard, "Yesterday, we _____ a great film.", "Past simple of 'watch' is 'watched'.", "watched", "watch", "watching", "watches");
        AddQ(CefrLevel.A1, DifficultyLevel.Hard, "How _____ butter do we need?", "Use 'much' for uncountable nouns like butter.", "much", "many", "few", "long");
        AddQ(CefrLevel.A1, DifficultyLevel.Hard, "This laptop is mine, and that one is _____.", "Possessive pronoun for 'you' is 'yours'.", "yours", "your", "you", "yourselves");

        // A2 Easy
        AddQ(CefrLevel.A2, DifficultyLevel.Easy, "While I was studying, the phone _____.", "Past continuous interrupted by Past Simple.", "rang", "was ringing", "rings", "has rung");
        AddQ(CefrLevel.A2, DifficultyLevel.Easy, "If it rains tomorrow, we _____ at home.", "First conditional: If + present, will + infinitive.", "will stay", "stayed", "would stay", "stay");
        AddQ(CefrLevel.A2, DifficultyLevel.Easy, "She hasn't finished her homework _____.", "Use 'yet' in negative present perfect at the end of sentence.", "yet", "already", "since", "just");
        AddQ(CefrLevel.A2, DifficultyLevel.Easy, "Tokyo is the _____ city in Japan.", "Superlative of 'large' is 'largest'.", "largest", "larger", "more large", "most large");
        AddQ(CefrLevel.A2, DifficultyLevel.Easy, "You _____ wear a seatbelt while driving.", "Must indicates obligation/law.", "must", "might", "could", "would");

        // A2 Middle
        AddQ(CefrLevel.A2, DifficultyLevel.Middle, "Have you ever _____ to London?", "Present perfect uses past participle 'been'.", "been", "went", "go", "going");
        AddQ(CefrLevel.A2, DifficultyLevel.Middle, "He promised _____ me with the project.", "Promise is followed by to-infinitive.", "to help", "helping", "help", "helped");
        AddQ(CefrLevel.A2, DifficultyLevel.Middle, "She enjoys _____ books in her free time.", "Enjoy is followed by gerund (-ing).", "reading", "to read", "read", "reads");
        AddQ(CefrLevel.A2, DifficultyLevel.Middle, "The train leaves _____ 9:30 AM.", "Use 'at' for precise times.", "at", "in", "on", "for");
        AddQ(CefrLevel.A2, DifficultyLevel.Middle, "This problem is _____ difficult than the last one.", "Comparative of multi-syllable adjective uses 'more'.", "more", "most", "much", "as");

        // A2 Hard
        AddQ(CefrLevel.A2, DifficultyLevel.Hard, "I have lived in Tashkent _____ 2018.", "Use 'since' for a starting point in time.", "since", "for", "during", "from");
        AddQ(CefrLevel.A2, DifficultyLevel.Hard, "They _____ to Paris twice this year.", "Present perfect for life experiences up to now.", "have traveled", "travel", "traveled", "are traveling");
        AddQ(CefrLevel.A2, DifficultyLevel.Hard, "The car _____ repaired yesterday by the mechanic.", "Passive voice past simple: was/were + past participle.", "was", "is", "were", "been");
        AddQ(CefrLevel.A2, DifficultyLevel.Hard, "You don't need to come if you _____ want to.", "Present simple negative auxiliary 'don't'.", "don't", "doesn't", "didn't", "won't");
        AddQ(CefrLevel.A2, DifficultyLevel.Hard, "He asked me where I _____.", "Reported speech past shift.", "lived", "live", "am living", "have lived");

        // B1 Easy
        AddQ(CefrLevel.B1, DifficultyLevel.Easy, "By the time we arrived at the cinema, the movie _____.", "Past perfect for action before another past action.", "had started", "started", "has started", "was starting");
        AddQ(CefrLevel.B1, DifficultyLevel.Easy, "If I _____ enough money, I would buy a new car.", "Second conditional: If + past simple, would + infinitive.", "had", "have", "would have", "will have");
        AddQ(CefrLevel.B1, DifficultyLevel.Easy, "The person _____ called you earlier left a message.", "Relative pronoun for people is 'who' or 'that'.", "who", "which", "whose", "whom");
        AddQ(CefrLevel.B1, DifficultyLevel.Easy, "We had to cancel the picnic _____ the heavy rain.", "Use 'because of' before a noun phrase.", "because of", "because", "due", "despite");
        AddQ(CefrLevel.B1, DifficultyLevel.Easy, "She is looking forward to _____ her grandparents.", "Look forward to is followed by gerund (-ing).", "visiting", "visit", "visited", "to visit");

        // B1 Middle
        AddQ(CefrLevel.B1, DifficultyLevel.Middle, "Neither John _____ Sarah knew the answer.", "Correlative conjunctions: Neither...nor.", "nor", "or", "and", "but");
        AddQ(CefrLevel.B1, DifficultyLevel.Middle, "I wish I _____ speak French fluently.", "Wish + past simple for present regrets.", "could", "can", "will", "would can");
        AddQ(CefrLevel.B1, DifficultyLevel.Middle, "He was accused of _____ money from the company.", "Preposition 'of' followed by gerund.", "stealing", "steal", "to steal", "stolen");
        AddQ(CefrLevel.B1, DifficultyLevel.Middle, "Although it was freezing outside, they _____ for a walk.", "Conjunction 'although' shows contrast.", "went", "go", "had gone", "were going");
        AddQ(CefrLevel.B1, DifficultyLevel.Middle, "You had better _____ your doctor soon.", "'Had better' is followed by bare infinitive.", "see", "to see", "seeing", "saw");

        // B1 Hard
        AddQ(CefrLevel.B1, DifficultyLevel.Hard, "The novel, _____ was written in 1925, is a classic.", "Non-defining relative clause for things uses 'which'.", "which", "that", "where", "what");
        AddQ(CefrLevel.B1, DifficultyLevel.Hard, "Unless you _____ hard, you won't pass the exam.", "Unless means 'if not', followed by positive verb.", "study", "don't study", "will study", "studied");
        AddQ(CefrLevel.B1, DifficultyLevel.Hard, "She succeeded in _____ the project ahead of deadline.", "Succeed in is followed by gerund.", "completing", "complete", "to complete", "completed");
        AddQ(CefrLevel.B1, DifficultyLevel.Hard, "He acts as if he _____ everything.", "Subjunctive / hypothetical past: 'as if he knew/were'.", "knew", "knows", "has known", "will know");
        AddQ(CefrLevel.B1, DifficultyLevel.Hard, "Hardly _____ entered the room when the alarm sounded.", "Inversion with 'hardly had I'.", "had I", "I had", "did I", "was I");

        // B2 Easy
        AddQ(CefrLevel.B2, DifficultyLevel.Easy, "Had I known about the meeting, I _____ attended.", "Third conditional inverted: Had I known... I would have.", "would have", "will have", "had", "would");
        AddQ(CefrLevel.B2, DifficultyLevel.Easy, "It's high time you _____ looking for a job.", "'It's high time' is followed by past simple.", "started", "start", "have started", "will start");
        AddQ(CefrLevel.B2, DifficultyLevel.Easy, "The building is believed to _____ destroyed in the fire.", "Passive reporting structure: to have been + past participle.", "have been", "be", "was", "being");
        AddQ(CefrLevel.B2, DifficultyLevel.Easy, "In spite of _____ exhausted, she continued working.", "'In spite of' takes a gerund.", "being", "she was", "be", "been");
        AddQ(CefrLevel.B2, DifficultyLevel.Easy, "No sooner had we left the house _____ it began to pour.", "Correlative structure: No sooner... than.", "than", "when", "then", "that");

        // B2 Middle & Hard
        AddQ(CefrLevel.B2, DifficultyLevel.Middle, "He insisted that the manager _____ informed immediately.", "Subjunctive mood takes base form 'be'.", "be", "is", "was", "should");
        AddQ(CefrLevel.B2, DifficultyLevel.Middle, "The manager questioned whether the report was sufficiently _____.", "Adverb modifying adjective.", "thorough", "through", "though", "tough");
        AddQ(CefrLevel.B2, DifficultyLevel.Hard, "Seldom _____ such an astounding artistic performance.", "Inversion after negative adverbial 'seldom'.", "have I witnessed", "I witnessed", "I have witnessed", "did I witnessed");
        AddQ(CefrLevel.B2, DifficultyLevel.Hard, "He was notorious _____ making rash decisions.", "Collocation: notorious for.", "for", "with", "of", "about");
        AddQ(CefrLevel.B2, DifficultyLevel.Hard, "The policy is intended to mitigate the risks _____ with inflation.", "Participle clause: associated with.", "associated", "associating", "associate", "associates");

        // C1 Easy, Middle, Hard
        AddQ(CefrLevel.C1, DifficultyLevel.Easy, "Not until the late 19th century _____ widespread recognition.", "Inversion after 'Not until'.", "did the theory gain", "the theory gained", "gained the theory", "has the theory gained");
        AddQ(CefrLevel.C1, DifficultyLevel.Middle, "Her eloquence was such that she managed to _____ her opponents.", "Vocabulary: persuade / disarm.", "disarm", "disrupt", "discourage", "disband");
        AddQ(CefrLevel.C1, DifficultyLevel.Hard, "The contract contains several ambiguous clauses that are open to _____.", "Vocabulary: interpretation.", "interpretation", "interruption", "intercession", "interpolation");
        AddQ(CefrLevel.C1, DifficultyLevel.Hard, "Far from being a drawback, his frugal habits proved to be a major _____.", "Vocabulary: asset.", "asset", "liability", "detriment", "hazard");
        AddQ(CefrLevel.C1, DifficultyLevel.Hard, "Should you _____ any further assistance, do not hesitate to reach out.", "Inverted conditional: Should you require.", "require", "requiring", "required", "requires");

        // C2 Easy, Middle, Hard
        AddQ(CefrLevel.C2, DifficultyLevel.Easy, "Were it not for your unwavering support, the venture _____ failed.", "Inverted third conditional.", "would have", "will have", "should have", "had");
        AddQ(CefrLevel.C2, DifficultyLevel.Middle, "The author's prose is characterized by an exquisite balance of subtlety and _____.", "Advanced vocabulary: nuance / precision.", "poignancy", "procrastination", "perpetuation", "predilection");
        AddQ(CefrLevel.C2, DifficultyLevel.Hard, "He expressed deep remorse for having inadvertently _____ confidential information.", "Advanced vocabulary: divulged.", "divulged", "diverted", "diluted", "dissembled");
        AddQ(CefrLevel.C2, DifficultyLevel.Hard, "The new regulations effectively _____ the previous directives.", "Advanced vocabulary: supersede.", "supersede", "surpass", "suppress", "subvert");
        AddQ(CefrLevel.C2, DifficultyLevel.Hard, "Notwithstanding the harsh criticisms, her determination remained _____.", "Advanced vocabulary: indomitable.", "indomitable", "indolent", "ineffable", "incongruous");

        modelBuilder.Entity<Question>().HasData(questions);
        modelBuilder.Entity<AnswerOption>().HasData(options);
    }
}
