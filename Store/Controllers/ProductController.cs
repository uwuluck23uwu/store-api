using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClassLibrary.Models.Dto;

namespace Store.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    /// Get all products with pagination, search, and filters
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] int? sellerId = null)
    {
        var result = await _productService.GetAllAsync(pageNumber, pageSize, search, categoryId, sellerId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get product details by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _productService.GetByIdAsync(id);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get products by category
    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var result = await _productService.GetByCategoryAsync(categoryId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get products by seller
    [HttpGet("seller/{sellerId}")]
    public async Task<IActionResult> GetBySeller(int sellerId)
    {
        var result = await _productService.GetBySellerAsync(sellerId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Search products by keyword
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
            return BadRequest(new ResponseMessage(System.Net.HttpStatusCode.BadRequest, false, "Keyword is required"));

        var result = await _productService.SearchAsync(keyword);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Create new product (Seller or Admin)
    [HttpPost]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Create([FromForm] ProductCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // ใช้ userId = 8 สำหรับทดสอบ (ตาม User admin 01 ที่มีในฐานข้อมูล)
        int userId = User.FindFirst(ClaimTypes.NameIdentifier) != null
            ? int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "8")
            : 8;

        // Get seller ID from user ID
        // This should be handled in the service layer
        var result = await _productService.CreateAsync(userId, dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Update product (Seller owner or Admin)
    [HttpPut("{id}")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Update(int id, [FromForm] ProductUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _productService.UpdateAsync(id, userId, dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Delete product (Seller owner or Admin)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _productService.DeleteAsync(id, userId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }
}
