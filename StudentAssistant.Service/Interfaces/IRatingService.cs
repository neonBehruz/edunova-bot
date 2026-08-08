using StudentAssistant.Service.DTOs.Rating;

namespace StudentAssistant.Service.Interfaces;

public interface IRatingService
{
    Task<List<RatingDto>> GetTopRatingsAsync(int limit = 10);
    Task<RatingDto?> GetUserRatingAsync(long userId);
    Task UpdateRatingAsync(long userId, int scoreChange);
}
