using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClassLibrary.Models.Dto.Cultures;
using Store.Services.IServices;

namespace Store.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CultureController : ControllerBase
{
    private readonly ICultureService _cultureService;

    public CultureController(ICultureService cultureService)
    {
        _cultureService = cultureService;
    }

    /// Get all cultures
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _cultureService.GetAllAsync();

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get only active cultures
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveCultures()
    {
        var result = await _cultureService.GetActiveCulturesAsync();

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get culture by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _cultureService.GetByIdAsync(id);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Create new culture (Admin only)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromForm] CultureCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _cultureService.CreateAsync(dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Update culture (Admin only)
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromForm] CultureUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _cultureService.UpdateAsync(id, dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Delete culture (Admin only)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _cultureService.DeleteAsync(id);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Toggle culture active status (Admin only)
    [HttpPatch("{id}/toggle-status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _cultureService.ToggleActiveStatusAsync(id);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }
}
