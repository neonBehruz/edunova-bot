using Microsoft.EntityFrameworkCore;
using StudentAssistant.Data.Interfaces;
using StudentAssistant.Domain.Entities;
using StudentAssistant.Service.DTOs.Rating;
using StudentAssistant.Service.Interfaces;

namespace StudentAssistant.Service.Services;

public class RatingService : IRatingService
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<UserRating> _ratingRepository;

    public RatingService(
        IRepository<User> userRepository,
        IRepository<UserRating> ratingRepository)
    {
        _userRepository = userRepository;
        _ratingRepository = ratingRepository;
    }

    public async Task<List<RatingDto>> GetTopRatingsAsync(int limit = 10)
    {
        var users = await _userRepository.SelectAll()
            .OrderByDescending(u => u.RatingScore)
            .ThenByDescending(u => u.TotalCorrectAnswers)
            .Take(limit)
            .ToListAsync();

        int rank = 1;
        return users.Select(u => new RatingDto
        {
            Rank = rank++,
            UserId = u.Id,
            TelegramId = u.TelegramId,
            FirstName = u.FirstName,
            Username = u.Username,
            Level = u.CurrentLevel,
            RatingScore = u.RatingScore,
            TotalTests = u.TotalTestsTaken
        }).ToList();
    }

    public async Task<RatingDto?> GetUserRatingAsync(long userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        var higherUsersCount = await _userRepository.SelectAll(u => u.RatingScore > user.RatingScore).CountAsync();

        return new RatingDto
        {
            Rank = higherUsersCount + 1,
            UserId = user.Id,
            TelegramId = user.TelegramId,
            FirstName = user.FirstName,
            Username = user.Username,
            Level = user.CurrentLevel,
            RatingScore = user.RatingScore,
            TotalTests = user.TotalTestsTaken
        };
    }

    public async Task UpdateRatingAsync(long userId, int scoreChange)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return;

        user.RatingScore += scoreChange;
        if (user.RatingScore < 0) user.RatingScore = 0;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        var rating = await _ratingRepository.FirstOrDefaultAsync(r => r.UserId == userId);
        if (rating == null)
        {
            rating = new UserRating
            {
                UserId = userId,
                Score = user.RatingScore,
                Rank = 0
            };
            await _ratingRepository.AddAsync(rating);
        }
        else
        {
            rating.Score = user.RatingScore;
            _ratingRepository.Update(rating);
        }
        await _ratingRepository.SaveChangesAsync();
    }
}
