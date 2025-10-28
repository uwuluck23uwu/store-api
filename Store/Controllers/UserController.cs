using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClassLibrary.Models.Dto;
using Store.Services.IServices;

namespace Store.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { isSuccess = false, message = "User not found" });

            var result = await _userService.GetUserProfileAsync(userId.Value);

            if (result.TaskStatus)
                return Ok(result);
            else
                return NotFound(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            return StatusCode(500, new { isSuccess = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Update user profile (FirstName, LastName, PhoneNumber)
    /// </summary>
    [HttpPut("update")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { isSuccess = false, message = "Invalid data", errors = ModelState });

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { isSuccess = false, message = "User not found" });

            var result = await _userService.UpdateProfileAsync(userId.Value, dto);

            if (result.TaskStatus)
                return Ok(result);
            else
                return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile");
            return StatusCode(500, new { isSuccess = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Upload user avatar image
    /// </summary>
    [HttpPost("upload-avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile avatar)
    {
        try
        {
            if (avatar == null || avatar.Length == 0)
                return BadRequest(new { isSuccess = false, message = "No file uploaded" });

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(avatar.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { isSuccess = false, message = "Invalid file type. Only JPG, PNG, and GIF are allowed" });

            // Validate file size (max 5MB)
            if (avatar.Length > 5 * 1024 * 1024)
                return BadRequest(new { isSuccess = false, message = "File size must be less than 5MB" });

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { isSuccess = false, message = "User not found" });

            var result = await _userService.UploadAvatarAsync(userId.Value, avatar);

            if (result.TaskStatus)
                return Ok(result);
            else
                return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading avatar");
            return StatusCode(500, new { isSuccess = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Change user role (Customer, Seller, Admin)
    /// </summary>
    [HttpPost("change-role")]
    public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { isSuccess = false, message = "Invalid data", errors = ModelState });

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { isSuccess = false, message = "User not found" });

            var result = await _userService.ChangeRoleAsync(userId.Value, dto);

            if (result.TaskStatus)
                return Ok(result);
            else
                return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing role");
            return StatusCode(500, new { isSuccess = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get current user ID from JWT token
    /// </summary>
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
            return userId;

        return null;
    }
}
