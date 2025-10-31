using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClassLibrary.Models.Dto;

namespace Store.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppBannerController : ControllerBase
{
    private readonly IAppBannerService _bannerService;

    public AppBannerController(IAppBannerService bannerService)
    {
        _bannerService = bannerService;
    }

    /// Get all banners (Admin only)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _bannerService.GetAllAsync();

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get active banners (Public)
    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var result = await _bannerService.GetActiveAsync();

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get banner by ID (Admin only)
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _bannerService.GetByIdAsync(id);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Create new banner (Admin only)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromForm] AppBannerCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _bannerService.CreateAsync(dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Update banner (Admin only)
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromForm] AppBannerUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _bannerService.UpdateAsync(id, dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Delete banner (Admin only)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _bannerService.DeleteAsync(id);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Toggle banner active status (Admin only)
    [HttpPatch("{id}/toggle-status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _bannerService.ToggleActiveStatusAsync(id);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Update banner display order (Admin only)
    [HttpPatch("{id}/display-order")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateDisplayOrder(int id, [FromBody] int displayOrder)
    {
        var result = await _bannerService.UpdateDisplayOrderAsync(id, displayOrder);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }
}
