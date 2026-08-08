using StudentAssistant.Service.DTOs.Users;

namespace StudentAssistant.Service.Interfaces;

public interface IUserService
{
    Task<UserDto> GetOrCreateUserAsync(long telegramId, string firstName, string? lastName, string? username);
    Task<UserDto?> GetByTelegramIdAsync(long telegramId);
    Task<UserDto?> GetByIdAsync(long id);
    Task<UserDto> UpdateUserAsync(UserDto userDto);
}
