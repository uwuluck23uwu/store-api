using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClassLibrary.Models.Dto;

namespace Store.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// Get all my orders (customer view)
    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _orderService.GetByUserIdAsync(userId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get order details by ID (owner, seller, or admin)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _orderService.GetByIdAsync(id);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get orders for a seller (seller or admin only)
    [HttpGet("seller/{sellerId}")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> GetBySeller(int sellerId)
    {
        var result = await _orderService.GetBySellerIdAsync(sellerId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get all orders (admin only)
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllOrders()
    {
        var result = await _orderService.GetAllOrdersAsync();

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Create new order
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _orderService.CreateAsync(userId, dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Update order status (seller or admin)
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatusUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _orderService.UpdateStatusAsync(id, dto.Status, userId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Cancel order (owner only, must be Pending or Confirmed)
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _orderService.CancelOrderAsync(id, userId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }
}
