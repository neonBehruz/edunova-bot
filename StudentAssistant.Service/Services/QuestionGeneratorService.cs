using Microsoft.EntityFrameworkCore;
using StudentAssistant.Data.Interfaces;
using StudentAssistant.Domain.Entities;
using StudentAssistant.Domain.Enums;
using StudentAssistant.Service.Interfaces;

namespace StudentAssistant.Service.Services;

public class QuestionGeneratorService : IQuestionGeneratorService
{
    private readonly IRepository<Question> _questionRepository;
    private readonly IRepository<QuestionHistory> _historyRepository;

    public QuestionGeneratorService(
        IRepository<Question> questionRepository,
        IRepository<QuestionHistory> historyRepository)
    {
        _questionRepository = questionRepository;
        _historyRepository = historyRepository;
    }

    public async Task<List<Question>> GenerateQuestionsAsync(long userId, long subjectId, CefrLevel level, DifficultyLevel difficulty, int count)
    {
        // 1. Fetch candidate questions matching Subject, Level, and Difficulty
        var candidateQuestions = await _questionRepository.SelectAll(q =>
                q.SubjectId == subjectId &&
                q.Level == level &&
                q.Difficulty == difficulty)
            .Include(q => q.Options)
            .ToListAsync();

        if (!candidateQuestions.Any())
        {
            candidateQuestions = await _questionRepository.SelectAll(q =>
                    q.SubjectId == subjectId &&
                    q.Level == level)
                .Include(q => q.Options)
                .ToListAsync();
        }

        if (!candidateQuestions.Any())
        {
            candidateQuestions = await _questionRepository.SelectAll(q => q.SubjectId == subjectId)
                .Include(q => q.Options)
                .ToListAsync();
        }

        // 2. Fetch history for this user
        var userHistories = await _historyRepository.SelectAll(h => h.UserId == userId)
            .ToDictionaryAsync(h => h.QuestionId);

        // 3. Prioritize questions never answered or answered least recently
        var selected = candidateQuestions
            .OrderBy(q => userHistories.ContainsKey(q.Id) ? userHistories[q.Id].TimesAnswered : 0)
            .ThenBy(_ => Guid.NewGuid())
            .Take(count)
            .ToList();

        // 4. Fallback Auto Question Generator if more questions needed
        if (selected.Count < count)
        {
            int needed = count - selected.Count;
            var dynamicGenerated = GenerateDynamicQuestions(subjectId, level, difficulty, needed, selected.Count + 100);
            selected.AddRange(dynamicGenerated);
        }

        return selected;
    }

    private static List<Question> GenerateDynamicQuestions(long subjectId, CefrLevel level, DifficultyLevel difficulty, int count, long startId)
    {
        var list = new List<Question>();
        var random = new Random();

        var templates = GetTemplatesForLevel(level, difficulty);

        for (int i = 0; i < count; i++)
        {
            var tmpl = templates[random.Next(templates.Count)];
            long qId = startId + i + 1;

            var q = new Question
            {
                Id = qId,
                SubjectId = subjectId,
                Level = level,
                Difficulty = difficulty,
                Text = tmpl.Text,
                Explanation = tmpl.Explanation,
                Type = QuestionType.SingleChoice,
                Options = new List<AnswerOption>()
            };

            int order = 1;
            var allOptions = tmpl.WrongOptions.Select(w => (Text: w, IsCorrect: false)).ToList();
            allOptions.Insert(random.Next(allOptions.Count + 1), (Text: tmpl.CorrectOption, IsCorrect: true));

            foreach (var opt in allOptions)
            {
                q.Options.Add(new AnswerOption
                {
                    Id = qId * 10 + order,
                    QuestionId = qId,
                    Text = opt.Text,
                    IsCorrect = opt.IsCorrect,
                    Order = order++
                });
            }

            list.Add(q);
        }

        return list;
    }

    private static List<(string Text, string CorrectOption, string[] WrongOptions, string Explanation)> GetTemplatesForLevel(CefrLevel level, DifficultyLevel difficulty)
    {
        return level switch
        {
            CefrLevel.A1 => new List<(string, string, string[], string)>
            {
                ("She _____ to the park every morning.", "goes", new[] { "go", "going", "gone" }, "Third person singular present simple takes '-es'."),
                ("They _____ my best friends.", "are", new[] { "is", "am", "be" }, "Use 'are' with 'they'."),
                ("I _____ a doctor in Tashkent.", "am", new[] { "is", "are", "be" }, "Use 'am' with 'I'."),
                ("What time _____ the lesson start?", "does", new[] { "do", "is", "are" }, "Use 'does' for singular present questions."),
                ("We _____ have any classes on Sunday.", "don't", new[] { "doesn't", "not", "aren't" }, "Use 'don't' for plural present negative.")
            },
            CefrLevel.A2 => new List<(string, string, string[], string)>
            {
                ("While I was watching TV, the phone _____.", "rang", new[] { "was ringing", "rings", "has rung" }, "Past continuous interrupted by Past Simple."),
                ("She has lived here _____ three years.", "for", new[] { "since", "during", "from" }, "Use 'for' for duration of time."),
                ("If it rains, we _____ inside.", "will stay", new[] { "stayed", "would stay", "stay" }, "First conditional: If + present, will + verb."),
                ("Have you ever _____ sushi?", "eaten", new[] { "ate", "eat", "eating" }, "Present perfect uses past participle 'eaten'."),
                ("This test is _____ than the previous one.", "easier", new[] { "easy", "more easy", "easiest" }, "Comparative of easy is easier.")
            },
            CefrLevel.B1 => new List<(string, string, string[], string)>
            {
                ("If I _____ more time, I would learn Spanish.", "had", new[] { "have", "would have", "will have" }, "Second conditional: If + past simple, would + verb."),
                ("By the time we arrived, the show _____.", "had ended", new[] { "ended", "has ended", "was ending" }, "Past perfect for action before another past action."),
                ("She is really looking forward to _____ you.", "meeting", new[] { "meet", "to meet", "met" }, "Look forward to is followed by gerund (-ing)."),
                ("You had better _____ your jacket.", "take", new[] { "to take", "taking", "took" }, "'Had better' takes bare infinitive."),
                ("Neither Tom _____ David came to the party.", "nor", new[] { "or", "and", "but" }, "Correlative conjunction: Neither... nor.")
            },
            CefrLevel.B2 => new List<(string, string, string[], string)>
            {
                ("Had I known the truth, I _____ differently.", "would have acted", new[] { "will act", "acted", "would act" }, "Inverted third conditional."),
                ("It is vital that he _____ informed immediately.", "be", new[] { "is", "was", "should be" }, "Subjunctive mood takes base verb 'be'."),
                ("Seldom _____ such a remarkable achievement.", "have we witnessed", new[] { "we witnessed", "we have witnessed", "did we witnessed" }, "Inversion after negative adverbial 'seldom'."),
                ("In spite of _____ tired, she kept working.", "being", new[] { "she was", "be", "been" }, "In spite of is followed by gerund."),
                ("He acts as though he _____ everything.", "knew", new[] { "knows", "has known", "will know" }, "Hypothetical past after 'as though'.")
            },
            CefrLevel.C1 => new List<(string, string, string[], string)>
            {
                ("Not until the decision was made _____ the consequences.", "did they realize", new[] { "they realized", "realized they", "they have realized" }, "Inversion after 'Not until'."),
                ("Her argument was so compelling that it _____ all doubts.", "dispelled", new[] { "disrupted", "disbanded", "displaced" }, "Advanced vocabulary: dispelled."),
                ("Should you _____ any further clarification, please inform us.", "require", new[] { "requiring", "required", "requires" }, "Inverted conditional with Should."),
                ("The agreement is open to multiple _____.", "interpretations", new[] { "interruptions", "intercessions", "interpolations" }, "Advanced vocabulary: interpretations."),
                ("His frugal habits proved to be a valuable _____.", "asset", new[] { "liability", "detriment", "hazard" }, "Advanced vocabulary: asset.")
            },
            _ => new List<(string, string, string[], string)>
            {
                ("Were it not for your assistance, the project _____ failed.", "would have", new[] { "will have", "should have", "had" }, "Inverted third conditional."),
                ("He expressed deep regret for having inadvertently _____ secret data.", "divulged", new[] { "diverted", "diluted", "dissembled" }, "Advanced vocabulary: divulged."),
                ("The new framework effectively _____ the outdated legacy protocol.", "supersedes", new[] { "surpasses", "suppresses", "subverts" }, "Advanced vocabulary: supersedes."),
                ("Her dedication to research remains _____.", "indomitable", new[] { "indolent", "ineffable", "incongruous" }, "Advanced vocabulary: indomitable."),
                ("The prose strikes a delicate balance between subtlety and _____.", "nuance", new[] { "poignancy", "procrastination", "predilection" }, "Advanced vocabulary: nuance.")
            }
        };
    }
}
