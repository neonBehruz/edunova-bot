using StudentAssistant.Domain.Entities;
using StudentAssistant.Domain.Enums;

namespace StudentAssistant.Service.Interfaces;

public interface IQuestionGeneratorService
{
    Task<List<Question>> GenerateQuestionsAsync(long userId, long subjectId, CefrLevel level, DifficultyLevel difficulty, int count);
}
