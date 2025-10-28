using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClassLibrary.Models.Dto;

namespace Store.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    /// Get my shopping cart
    [HttpGet]
    public async Task<IActionResult> GetMyCart()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _cartService.GetByUserIdAsync(userId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get cart item count
    [HttpGet("count")]
    public async Task<IActionResult> GetCount()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        int count = await _cartService.GetCartCountAsync(userId);

        return Ok(new ResponseData(
            System.Net.HttpStatusCode.OK,
            true,
            "Cart count retrieved",
            new { Count = count }
        ));
    }

    /// Add product to cart
    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _cartService.AddToCartAsync(userId, dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Update cart item quantity
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuantity(int id, [FromBody] UpdateCartItemDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _cartService.UpdateQuantityAsync(id, userId, dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Remove item from cart
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _cartService.RemoveFromCartAsync(id, userId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Clear entire cart
    [HttpDelete("clear")]
    public async Task<IActionResult> Clear()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _cartService.ClearCartAsync(userId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }
}
