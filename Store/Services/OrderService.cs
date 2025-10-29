using Microsoft.EntityFrameworkCore;
using ClassLibrary.Models.Data;
using ClassLibrary.Models.Dto;
using Store.Data;

namespace Store.Services;

public class OrderService : Service<Order>, IServices.IOrderService
{
    public OrderService(ApplicationDbContext db, AutoMapper.IMapper mapper)
        : base(db, mapper)
    {
    }

    public async Task<ResponseData> GetByUserIdAsync(int userId)
    {
        try
        {
            var orders = await _db.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Seller)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var orderDtos = _mapper.Map<List<OrderDTO>>(orders);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Orders retrieved successfully",
                orderDtos
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

    public async Task<ResponseData> GetByIdAsync(int id)
    {
        try
        {
            var order = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Seller)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return new ResponseData(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Order not found",
                null
            );
            }

            var orderDto = _mapper.Map<OrderDetailDTO>(order);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Order retrieved successfully",
                orderDto
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

    public async Task<ResponseData> GetBySellerIdAsync(int sellerId)
    {
        try
        {
            var orderItems = await _db.OrderItems
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.User)
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .Where(oi => oi.SellerId == sellerId)
                .OrderByDescending(oi => oi.Order.CreatedAt)
                .ToListAsync();

            var orderItemDtos = _mapper.Map<List<OrderItemDetailDTO>>(orderItems);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Seller orders retrieved successfully",
                orderItemDtos
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

    public async Task<ResponseData> GetAllOrdersAsync()
    {
        try
        {
            var orders = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Seller)
                .Include(o => o.Payment)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var orderDtos = _mapper.Map<List<OrderDetailDTO>>(orders);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "All orders retrieved successfully",
                orderDtos
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

    public async Task<ResponseData> CreateAsync(int userId, OrderCreateDTO dto)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            // Validate all products and check stock
            var productIds = dto.Items.Select(i => i.ProductId).ToList();
            var products = await _db.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId);

            foreach (var item in dto.Items)
            {
                if (!products.ContainsKey(item.ProductId))
                {
                    await transaction.RollbackAsync();
                    return new ResponseData(
                        System.Net.HttpStatusCode.BadRequest,
                        false,
                        $"Product with ID {item.ProductId} not found",
                        null
                    );
                }

                var product = products[item.ProductId];

                if (!product.IsActive)
                {
                    await transaction.RollbackAsync();
                    return new ResponseData(
                        System.Net.HttpStatusCode.BadRequest,
                        false,
                        $"Product {product.ProductName} is not available",
                        null
                    );
                }

                if (product.Stock < item.Quantity)
                {
                    await transaction.RollbackAsync();
                    return new ResponseData(
                        System.Net.HttpStatusCode.BadRequest,
                        false,
                        $"Insufficient stock for {product.ProductName}. Available: {product.Stock}",
                        null
                    );
                }
            }

            // Generate order number
            var orderNumber = await GenerateOrderNumberAsync();

            // Calculate totals
            decimal totalPrice = 0;
            foreach (var item in dto.Items)
            {
                var product = products[item.ProductId];
                totalPrice += (product.Price.HasValue ? product.Price.Value : 0) * item.Quantity;
            }

            // Create order
            var order = new Order
            {
                UserId = userId,
                OrderNumber = orderNumber,
                TotalAmount = totalPrice,
                Status = "Pending",
                Note = dto.Note,
                OrderDate = DateTime.Now,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();

            // Create order items and update stock
            foreach (var item in dto.Items)
            {
                var product = products[item.ProductId];
                decimal productPrice = product.Price ?? 0;

                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    SellerId = product.SellerId,
                    Quantity = item.Quantity,
                    UnitPrice = productPrice,
                    Price = productPrice,
                    TotalPrice = productPrice * item.Quantity,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _db.OrderItems.AddAsync(orderItem);

                // Update product stock
                product.Stock -= item.Quantity;
                product.UpdatedAt = DateTime.Now;
            }

            await _db.SaveChangesAsync();

            // Create Payment record
            var payment = new Payment
            {
                OrderId = order.OrderId,
                Method = dto.PaymentMethod,
                Amount = totalPrice,
                Status = "Pending",
                ReferenceCode = null,
                PaidAt = null
            };
            await _db.Payments.AddAsync(payment);
            await _db.SaveChangesAsync();

            // Calculate and create SellerRevenue for each seller
            var sellerGroups = dto.Items
                .GroupBy(i => products[i.ProductId].SellerId)
                .Select(g => new
                {
                    SellerId = g.Key,
                    GrossAmount = g.Sum(i => (products[i.ProductId].Price ?? 0) * i.Quantity)
                });

            foreach (var sellerGroup in sellerGroups)
            {
                decimal commissionRate = 0; // Default: no commission (can be configured later)
                decimal commissionAmount = sellerGroup.GrossAmount * (commissionRate / 100);
                decimal netAmount = sellerGroup.GrossAmount - commissionAmount;

                var sellerRevenue = new SellerRevenue
                {
                    OrderId = order.OrderId,
                    SellerId = sellerGroup.SellerId,
                    GrossAmount = sellerGroup.GrossAmount,
                    CommissionRate = commissionRate,
                    CommissionAmount = commissionAmount,
                    NetAmount = netAmount,
                    Status = "Pending",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _db.SellerRevenues.AddAsync(sellerRevenue);
            }
            await _db.SaveChangesAsync();

            // Clear cart items for ordered products
            var cartItems = await _db.Carts
                .Where(c => c.UserId == userId && productIds.Contains(c.ProductId))
                .ToListAsync();

            _db.Carts.RemoveRange(cartItems);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            // Return created order
            return await GetByIdAsync(order.OrderId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new ResponseData(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}",
                null
            );
        }
    }

    public async Task<ResponseMessage> UpdateStatusAsync(int id, string status, int userId)
    {
        try
        {
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return new ResponseMessage(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Order not found"
            );
            }

            // Validate status transition
            var validStatuses = new[] { "Pending", "Confirmed", "Shipped", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(status))
            {
                return new ResponseMessage(
                System.Net.HttpStatusCode.BadRequest,
                false,
                "Invalid status"
            );
            }

            // Check if seller owns any items in the order
            var user = await _db.Users.Include(u => u.Seller).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user?.Role != "Admin")
            {
                if (user?.Seller == null)
                {
                    return new ResponseMessage(
                System.Net.HttpStatusCode.Forbidden,
                false,
                "You don't have permission to update this order"
            );
                }

                var hasSellerId = order.OrderItems.Any(oi => oi.SellerId == user.Seller.SellerId);
                if (!hasSellerId)
                {
                    return new ResponseMessage(
                System.Net.HttpStatusCode.Forbidden,
                false,
                "You don't have permission to update this order"
            );
                }
            }

            order.Status = status;
            order.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Order status updated successfully"
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

    public async Task<ResponseMessage> CancelOrderAsync(int id, int userId)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return new ResponseMessage(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Order not found"
            );
            }

            // Check ownership
            if (order.UserId != userId)
            {
                await transaction.RollbackAsync();
                return new ResponseMessage(
                System.Net.HttpStatusCode.Forbidden,
                false,
                "You don't have permission to cancel this order"
            );
            }

            // Check if order can be cancelled
            if (order.Status != "Pending" && order.Status != "Confirmed")
            {
                await transaction.RollbackAsync();
                return new ResponseMessage(
                System.Net.HttpStatusCode.BadRequest,
                false,
                "Order cannot be cancelled in current status"
            );
            }

            // Restore stock
            foreach (var orderItem in order.OrderItems)
            {
                orderItem.Product.Stock += orderItem.Quantity;
                orderItem.Product.UpdatedAt = DateTime.Now;
            }

            order.Status = "Cancelled";
            order.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Order cancelled successfully"
            );
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new ResponseMessage(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}"
            );
        }
    }

    private async Task<string> GenerateOrderNumberAsync()
    {
        string orderNumber;
        bool isUnique;

        do
        {
            // Generate format: ORD-yyyyMMdd-Random4Digits
            var datePart = DateTime.Now.ToString("yyyyMMdd");
            var random = new Random();
            var randomPart = random.Next(1000, 9999);
            orderNumber = $"ORD-{datePart}-{randomPart}";

            // Check if order number already exists
            isUnique = !await _db.Orders.AnyAsync(o => o.OrderNumber == orderNumber);
        } while (!isUnique);

        return orderNumber;
    }
}
