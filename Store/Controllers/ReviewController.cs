using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClassLibrary.Models.Dto;

namespace Store.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    /// Get all reviews for a product
    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetByProduct(int productId)
    {
        var result = await _reviewService.GetByProductIdAsync(productId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Create a review
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ReviewCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        // Check if user can review only if orderId is provided
        if (dto.OrderId.HasValue)
        {
            bool canReview = await _reviewService.CanReviewAsync(userId, dto.ProductId, dto.OrderId.Value);

            if (!canReview)
            {
                return BadRequest(new ResponseMessage(
                    System.Net.HttpStatusCode.BadRequest,
                    false,
                    "Cannot review: Order must be delivered or product not purchased"
                ));
            }
        }

        var result = await _reviewService.CreateAsync(userId, dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Update review (owner only)
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] ReviewUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _reviewService.UpdateAsync(id, userId, dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Delete review (owner or admin)
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _reviewService.DeleteAsync(id, userId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }
}
