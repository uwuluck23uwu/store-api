using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClassLibrary.Models.Dto;

namespace Store.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SellerController : ControllerBase
{
    private readonly ISellerService _sellerService;
    private readonly IProductService _productService;

    public SellerController(ISellerService sellerService, IProductService productService)
    {
        _sellerService = sellerService;
        _productService = productService;
    }

    /// Get all sellers
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _sellerService.GetAllAsync();

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get seller profile by seller ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfile(int id)
    {
        var result = await _sellerService.GetProfileAsync(id);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get seller by user ID
    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var result = await _sellerService.GetByUserIdAsync(userId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get products by seller ID
    [HttpGet("{id}/products")]
    public async Task<IActionResult> GetProducts(int id)
    {
        var result = await _productService.GetBySellerAsync(id);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Register as a seller (requires authentication)
    [HttpPost]
    // [Authorize] // ปิดชั่วคราวเพื่อทดสอบ
    public async Task<IActionResult> Register([FromBody] SellerCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // ใช้ userId = 1 สำหรับทดสอบ
        int userId = User.FindFirst(ClaimTypes.NameIdentifier) != null
            ? int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1")
            : 1;

        var result = await _sellerService.RegisterSellerAsync(userId, dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Update seller profile (Seller owner or Admin)
    [HttpPut("{id}")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> UpdateProfile(int id, [FromBody] SellerUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _sellerService.UpdateProfileAsync(id, dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// <summary>
    /// Upload shop image for seller (requires Seller or Admin role)
    /// </summary>
    [HttpPost("upload-shop-image")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> UploadShopImage(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { isSuccess = false, message = "No file uploaded" });

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { isSuccess = false, message = "Invalid file type. Only JPG, PNG, and GIF are allowed" });

            // Validate file size (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { isSuccess = false, message = "File size must be less than 5MB" });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
                return Unauthorized(new { isSuccess = false, message = "User not found" });

            var result = await _sellerService.UploadShopImageAsync(userId, file);

            if (result.TaskStatus)
                return Ok(result);
            else
                return BadRequest(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { isSuccess = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Upload logo for seller (requires Seller or Admin role)
    /// </summary>
    [HttpPost("upload-logo")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> UploadLogo(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { isSuccess = false, message = "No file uploaded" });

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { isSuccess = false, message = "Invalid file type. Only JPG, PNG, and GIF are allowed" });

            // Validate file size (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { isSuccess = false, message = "File size must be less than 5MB" });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
                return Unauthorized(new { isSuccess = false, message = "User not found" });

            var result = await _sellerService.UploadLogoAsync(userId, file);

            if (result.TaskStatus)
                return Ok(result);
            else
                return BadRequest(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { isSuccess = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Upload QR Code for seller (requires Seller or Admin role)
    /// </summary>
    [HttpPost("upload-qrcode")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> UploadQRCode(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { isSuccess = false, message = "No file uploaded" });

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { isSuccess = false, message = "Invalid file type. Only JPG, PNG, and GIF are allowed" });

            // Validate file size (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { isSuccess = false, message = "File size must be less than 5MB" });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
                return Unauthorized(new { isSuccess = false, message = "User not found" });

            var result = await _sellerService.UploadQRCodeAsync(userId, file);

            if (result.TaskStatus)
                return Ok(result);
            else
                return BadRequest(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { isSuccess = false, message = ex.Message });
        }
    }
}
