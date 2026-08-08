using StudentAssistant.Domain.Enums;
using StudentAssistant.Service.DTOs.Questions;

namespace StudentAssistant.Service.Interfaces;

public interface IQuestionService
{
    Task<QuestionDto?> GetByIdAsync(long id);
    Task<IEnumerable<QuestionDto>> GetByLevelAndDifficultyAsync(CefrLevel level, DifficultyLevel difficulty);
}
