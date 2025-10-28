using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClassLibrary.Models.Dto;

namespace Store.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    /// Get all my addresses
    [HttpGet]
    public async Task<IActionResult> GetMyAddresses()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _addressService.GetByUserIdAsync(userId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get address by ID (owner only)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _addressService.GetByIdAsync(id);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Get default address
    [HttpGet("default")]
    public async Task<IActionResult> GetDefaultAddress()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _addressService.GetDefaultAddressAsync(userId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }

    /// Create new address
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddressCreateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _addressService.CreateAsync(userId, dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Update address (owner only)
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] AddressUpdateDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _addressService.UpdateAsync(id, userId, dto);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Set address as default
    [HttpPut("{id}/set-default")]
    public async Task<IActionResult> SetDefault(int id)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _addressService.SetDefaultAsync(userId, id);

        if (result.TaskStatus)
            return Ok(result);
        else
            return BadRequest(result);
    }

    /// Delete address (owner only)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _addressService.DeleteAsync(id, userId);

        if (result.TaskStatus)
            return Ok(result);
        else
            return NotFound(result);
    }
}
