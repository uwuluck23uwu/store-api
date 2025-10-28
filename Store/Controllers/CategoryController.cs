using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClassLibrary.Models.Dto;

namespace Store.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// Get all categories
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllAsync();

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get only active categories
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveCategories()
    {
        var result = await _categoryService.GetActiveCategoriesAsync();

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get category by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _categoryService.GetByIdAsync(id);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Create new category (Admin only)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromForm] CategoryCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _categoryService.CreateAsync(dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Update category (Admin only)
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromForm] CategoryUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _categoryService.UpdateAsync(id, dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Delete category (Admin only)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryService.DeleteAsync(id);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }
}
