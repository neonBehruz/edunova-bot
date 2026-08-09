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

        // 2. Fetch answered question IDs for THIS specific user from QuestionHistory
        var userAnsweredQuestionIds = await _historyRepository.SelectAll(h => h.UserId == userId && h.TimesAnswered > 0)
            .Select(h => h.QuestionId)
            .ToListAsync();

        // 3. Filter out questions already answered by THIS specific user
        var freshQuestions = candidateQuestions
            .Where(q => !userAnsweredQuestionIds.Contains(q.Id))
            .OrderBy(_ => Guid.NewGuid())
            .Take(count)
            .ToList();

        // 4. Dynamic question pool fallback to guarantee 10 easy questions per test
        if (freshQuestions.Count < count)
        {
            int needed = count - freshQuestions.Count;
            long maxExistingId = candidateQuestions.Any() ? candidateQuestions.Max(q => q.Id) : 1000;
            var dynamicQuestions = GenerateDynamicEasyQuestions(subjectId, level, difficulty, needed, maxExistingId + 5000);
            freshQuestions.AddRange(dynamicQuestions);
        }

        return freshQuestions;
    }

    private static List<Question> GenerateDynamicEasyQuestions(long subjectId, CefrLevel level, DifficultyLevel difficulty, int count, long startId)
    {
        var list = new List<Question>();
        var random = new Random();
        var pool = GetEasyQuestionPoolForSubject(subjectId);

        for (int i = 0; i < count; i++)
        {
            var item = pool[random.Next(pool.Count)];
            long qId = startId + i + 1;

            var q = new Question
            {
                Id = qId,
                SubjectId = subjectId,
                Level = level,
                Difficulty = difficulty,
                Text = item.Text,
                Explanation = item.Explanation,
                Type = QuestionType.SingleChoice,
                Options = new List<AnswerOption>()
            };

            var options = item.WrongOptions.Select(w => (Text: w, IsCorrect: false)).ToList();
            options.Insert(random.Next(options.Count + 1), (Text: item.CorrectOption, IsCorrect: true));

            int order = 1;
            foreach (var opt in options)
            {
                q.Options.Add(new AnswerOption
                {
                    Id = qId * 100 + order,
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

    private static List<(string Text, string CorrectOption, string[] WrongOptions, string Explanation)> GetEasyQuestionPoolForSubject(long subjectId)
    {
        return subjectId switch
        {
            3 => new List<(string, string, string[], string)> // Matematika
            {
                ("15 + 25 yig'indi nechaga teng?", "40", new[] { "35", "45", "50" }, "15 va 25 ning yig'indisi 40 bo'ladi."),
                ("100 - 45 ayirma nechaga teng?", "55", new[] { "45", "65", "50" }, "100 - 45 = 55."),
                ("7 * 8 ko'paytma nechaga teng?", "56", new[] { "48", "64", "54" }, "7 * 8 = 56."),
                ("81 : 9 bo'linma nechaga teng?", "9", new[] { "8", "7", "10" }, "81 bo'lingan 9 teng 9."),
                ("Kvadratning barcha tomonlari uzunligi qanday?", "Teng bo'ladi", new[] { "Har xil bo'ladi", "Faqat 2 tasi teng", "Noma'lum" }, "Kvadrat barcha tomonlari teng geometrik shakldir.")
            },
            2 => new List<(string, string, string[], string)> // Tarix
            {
                ("Amir Temur qaysi yili tug'ilgan?", "1336-yil", new[] { "1441-yil", "1370-yil", "1405-yil" }, "Amir Temur 1336-yil 9-aprelda tug'ilgan."),
                ("Alisher Navoiy qaysi yili tug'ilgan?", "1441-yil", new[] { "1336-yil", "1483-yil", "1207-yil" }, "Alisher Navoiy 1441-yil 9-fevralda tug'ilgan."),
                ("O'zbekiston Mustaqillik kuni qachon?", "31-avgust", new[] { "1-sentabr", "8-dekabr", "21-mart" }, "1991-yil 31-avgustda mustaqillik e'lon qilingan.")
            },
            1 => new List<(string, string, string[], string)> // Ingliz tili
            {
                ("Choose the correct verb: He _____ to school every day.", "goes", new[] { "go", "going", "gone" }, "Present simple third person takes 'goes'."),
                ("What is the plural of 'child'?", "children", new[] { "childs", "childrens", "childes" }, "'Child' forms irregular plural 'children'."),
                ("Choose the correct form: They _____ playing in the park.", "are", new[] { "is", "am", "be" }, "'They' takes verb 'are'.")
            },
            4 => new List<(string, string, string[], string)> // Ona tili va Adabiyot
            {
                ("O'zbek adabiy tili asoschisi kim?", "Alisher Navoiy", new[] { "Zahiriddin Muhammad Bobur", "Abdulla Qodiriy", "Cho'lpon" }, "Alisher Navoiy o'zbek adabiy tiliga asos solgan."),
                ("O'zbek alifbosida nechta unli harf bor?", "6 ta", new[] { "5 ta", "10 ta", "8 ta" }, "A, O, I, U, O', E 6 ta unli harflardir.")
            },
            _ => new List<(string, string, string[], string)>
            {
                ("Ushbu fandan berilgan eng sodda test savoli to'g'ri javobi qaysi?", "To'g'ri javob", new[] { "Noto'g'ri javob 1", "Noto'g'ri javob 2", "Noto'g'ri javob 3" }, "Savol test rejimida avtomatik shakllandi."),
                ("Mavzuni mustahkamlash uchun berilgan savolga to'g'ri variantni tanlang.", "A variant", new[] { "B variant", "C variant", "D variant" }, "A variant to'g'ri javob hisoblanadi.")
            }
        };
    }
}
