using StudentAssistant.Data.Interfaces;
using StudentAssistant.Domain.Entities;

using StudentAssistant.Service.DTOs.Users;
using StudentAssistant.Service.Interfaces;

namespace StudentAssistant.Service.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> GetOrCreateUserAsync(long telegramId, string firstName, string? lastName, string? username)
    {
        var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.TelegramId == telegramId);
        if (existingUser != null)
        {
            // Update info if changed
            existingUser.FirstName = firstName;
            existingUser.LastName = lastName;
            existingUser.Username = username;
            _userRepository.Update(existingUser);
            await _userRepository.SaveChangesAsync();
            return MapToDto(existingUser);
        }

        var newUser = new User
        {
            TelegramId = telegramId,
            FirstName = firstName,
            LastName = lastName,
            Username = username,
            Role = Domain.Enums.UserRole.Student,
            CurrentLevel = Domain.Enums.CefrLevel.A1
        };

        var created = await _userRepository.AddAsync(newUser);
        await _userRepository.SaveChangesAsync();

        return MapToDto(created);
    }

    public async Task<UserDto?> GetByTelegramIdAsync(long telegramId)
    {
        var user = await _userRepository.FirstOrDefaultAsync(u => u.TelegramId == telegramId);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto?> GetByIdAsync(long id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto> UpdateUserAsync(UserDto userDto)
    {
        var user = await _userRepository.GetByIdAsync(userDto.Id);
        if (user == null) throw new InvalidOperationException("User not found");

        user.FirstName = userDto.FirstName;
        user.LastName = userDto.LastName;
        user.Username = userDto.Username;
        user.CurrentLevel = userDto.CurrentLevel;
        user.RatingScore = userDto.RatingScore;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return MapToDto(user);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            TelegramId = user.TelegramId,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            CurrentLevel = user.CurrentLevel,
            RatingScore = user.RatingScore,
            TotalTestsTaken = user.TotalTestsTaken,
            TotalCorrectAnswers = user.TotalCorrectAnswers,
            CreatedAt = user.CreatedAt
        };
    }
}
