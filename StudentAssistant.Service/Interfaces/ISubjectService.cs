using StudentAssistant.Service.DTOs.Subjects;

namespace StudentAssistant.Service.Interfaces;

public interface ISubjectService
{
    Task<IEnumerable<SubjectDto>> GetAllAsync();
    Task<SubjectDto?> GetByIdAsync(long id);
}
