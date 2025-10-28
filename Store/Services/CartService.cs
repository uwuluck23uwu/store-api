using Microsoft.EntityFrameworkCore;
using ClassLibrary.Models.Data;
using ClassLibrary.Models.Dto;
using Store.Data;

namespace Store.Services;

public class CartService : Service<Cart>, IServices.ICartService
{
    public CartService(ApplicationDbContext db, AutoMapper.IMapper mapper)
        : base(db, mapper)
    {
    }

    public async Task<ResponseData> GetByUserIdAsync(int userId)
    {
        try
        {
            var cartItems = await _db.Carts
                .Include(c => c.Product)
                    .ThenInclude(p => p.Seller)
                .Include(c => c.Product)
                    .ThenInclude(p => p.Category)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var cartDtos = _mapper.Map<List<CartItemDTO>>(cartItems);

            // Calculate total
            var total = cartItems.Sum(c => c.Product.Price * c.Quantity);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Cart retrieved successfully",
                new
                {
                    Items = cartDtos,
                    Total = total,
                    ItemCount = cartItems.Sum(c => c.Quantity)
                }
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}",
                null
            );
        }
    }

    public async Task<int> GetCartCountAsync(int userId)
    {
        try
        {
            var count = await _db.Carts
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity);

            return count;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public async Task<ResponseMessage> AddToCartAsync(int userId, AddToCartDTO dto)
    {
        try
        {
            // Check if product exists and is active
            var product = await _db.Products.FindAsync(dto.ProductId);
            if (product == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Product not found"
                );
            }

            if (!product.IsActive)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.BadRequest,
                    false,
                    "Product is not available"
                );
            }

            // Check if product has enough stock
            if (product.Stock < dto.Quantity)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.BadRequest,
                    false,
                    $"Insufficient stock. Available: {product.Stock}"
                );
            }

            // Check if product is already in cart
            var existingCart = await _db.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);

            if (existingCart != null)
            {
                // Update quantity
                var newQuantity = existingCart.Quantity + dto.Quantity;

                if (product.Stock < newQuantity)
                {
                    return new ResponseMessage(
                        System.Net.HttpStatusCode.BadRequest,
                        false,
                        $"Insufficient stock. Available: {product.Stock}, Current in cart: {existingCart.Quantity}"
                    );
                }

                existingCart.Quantity = newQuantity;
                existingCart.UpdatedAt = DateTime.Now;
            }
            else
            {
                // Add new cart item
                var cart = new Cart
                {
                    UserId = userId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _db.Carts.AddAsync(cart);
            }

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Product added to cart successfully"
            );
        }
        catch (Exception ex)
        {
            return new ResponseMessage(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}"
            );
        }
    }

    public async Task<ResponseMessage> UpdateQuantityAsync(int id, int userId, UpdateCartItemDTO dto)
    {
        try
        {
            var cart = await _db.Carts
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.CartId == id);

            if (cart == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Cart item not found"
                );
            }

            // Check ownership
            if (cart.UserId != userId)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.Forbidden,
                    false,
                    "You don't have permission to modify this cart item"
                );
            }

            // Check if product has enough stock
            if (cart.Product.Stock < dto.Quantity)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.BadRequest,
                    false,
                    $"Insufficient stock. Available: {cart.Product.Stock}"
                );
            }

            cart.Quantity = dto.Quantity;
            cart.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Cart quantity updated successfully"
            );
        }
        catch (Exception ex)
        {
            return new ResponseMessage(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}"
            );
        }
    }

    public async Task<ResponseMessage> RemoveFromCartAsync(int id, int userId)
    {
        try
        {
            var cart = await _db.Carts.FindAsync(id);

            if (cart == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Cart item not found"
                );
            }

            // Check ownership
            if (cart.UserId != userId)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.Forbidden,
                    false,
                    "You don't have permission to remove this cart item"
                );
            }

            _db.Carts.Remove(cart);
            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Cart item removed successfully"
            );
        }
        catch (Exception ex)
        {
            return new ResponseMessage(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}"
            );
        }
    }

    public async Task<ResponseMessage> ClearCartAsync(int userId)
    {
        try
        {
            var cartItems = await _db.Carts
                .Where(c => c.UserId == userId)
                .ToListAsync();

            _db.Carts.RemoveRange(cartItems);
            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Cart cleared successfully"
            );
        }
        catch (Exception ex)
        {
            return new ResponseMessage(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}"
            );
        }
    }
}
