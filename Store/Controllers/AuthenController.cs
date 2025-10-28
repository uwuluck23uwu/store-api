using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClassLibrary.Models.Dto;

namespace Store.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenController : ControllerBase
{
    private readonly IAuthenService _authenService;

    public AuthenController(IAuthenService authenService)
    {
        _authenService = authenService;
    }

    /// Register a new user
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authenService.RegisterAsync(dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Login to the system
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authenService.LoginAsync(dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return Unauthorized(result);
    }

    /// Refresh JWT token
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO dto)
    {
        var result = await _authenService.RefreshTokenAsync(dto.RefreshToken);

        if (result.TaskStatus)
            return Ok(result);
        else
            return Unauthorized(result);
    }

    /// Logout (revoke refresh token)
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _authenService.LogoutAsync(userId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Get current user profile
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _authenService.GetProfileAsync(userId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Update user profile
    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _authenService.UpdateProfileAsync(userId, dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Change password
    [HttpPut("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _authenService.ChangePasswordAsync(userId, dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }
}
