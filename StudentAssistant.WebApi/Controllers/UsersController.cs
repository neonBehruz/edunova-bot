using Microsoft.AspNetCore.Mvc;
using StudentAssistant.Service.DTOs.Users;
using StudentAssistant.Service.Interfaces;
using StudentAssistant.WebApi.Models;

namespace StudentAssistant.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound(ApiResponse<UserDto>.Fail("User not found"));
        return Ok(ApiResponse<UserDto>.Ok(user));
    }

    [HttpGet("telegram/{telegramId}")]
    public async Task<IActionResult> GetByTelegramId(long telegramId)
    {
        var user = await _userService.GetByTelegramIdAsync(telegramId);
        if (user == null) return NotFound(ApiResponse<UserDto>.Fail("User not found"));
        return Ok(ApiResponse<UserDto>.Ok(user));
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrGet([FromBody] CreateUserDto dto)
    {
        var user = await _userService.GetOrCreateUserAsync(dto.TelegramId, dto.FirstName, dto.LastName, dto.Username);
        return Ok(ApiResponse<UserDto>.Ok(user, "User processed successfully"));
    }
}
