using Microsoft.AspNetCore.Mvc;
using StudentAssistant.Service.DTOs.Rating;
using StudentAssistant.Service.Interfaces;
using StudentAssistant.WebApi.Models;

namespace StudentAssistant.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [HttpGet("top")]
    public async Task<IActionResult> GetTop([FromQuery] int limit = 10)
    {
        var ratings = await _ratingService.GetTopRatingsAsync(limit);
        return Ok(ApiResponse<List<RatingDto>>.Ok(ratings));
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserRating(long userId)
    {
        var rating = await _ratingService.GetUserRatingAsync(userId);
        if (rating == null) return NotFound(ApiResponse<RatingDto>.Fail("Rating not found"));
        return Ok(ApiResponse<RatingDto>.Ok(rating));
    }
}
