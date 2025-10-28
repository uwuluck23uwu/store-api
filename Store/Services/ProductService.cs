using Microsoft.EntityFrameworkCore;
using ClassLibrary.Models.Data;
using ClassLibrary.Models.Dto;
using Store.Data;

namespace Store.Services;

public class ProductService : Service<Product>, IServices.IProductService
{
    private readonly IServices.IImageService _imageService;

    public ProductService(ApplicationDbContext db, AutoMapper.IMapper mapper, IServices.IImageService imageService)
        : base(db, mapper)
    {
        _imageService = imageService;
    }

    public async Task<ResponseData> GetAllAsync(int pageNumber, int pageSize, string? search, int? categoryId, int? sellerId)
    {
        try
        {
            var query = _db.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Where(p => p.IsActive)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search) || p.Description.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (sellerId.HasValue)
            {
                query = query.Where(p => p.SellerId == sellerId.Value);
            }

            // Get total count
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Apply pagination
            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var productDtos = _mapper.Map<List<ProductDTO>>(products);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Products retrieved successfully",
                new
                {
                    Products = productDtos,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages
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

    public async Task<ResponseData> GetByIdAsync(int id)
    {
        try
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                    .ThenInclude(s => s.User)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return new ResponseData(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Product not found",
                null
            );
            }

            var productDto = _mapper.Map<ProductDetailDTO>(product);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Product retrieved successfully",
                productDto
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

    public async Task<ResponseData> GetByCategoryAsync(int categoryId)
    {
        try
        {
            var products = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var productDtos = _mapper.Map<List<ProductDTO>>(products);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Products retrieved successfully",
                productDtos
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

    public async Task<ResponseData> GetBySellerAsync(int sellerId)
    {
        try
        {
            var products = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Where(p => p.SellerId == sellerId && p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var productDtos = _mapper.Map<List<ProductDTO>>(products);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Products retrieved successfully",
                productDtos
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

    public async Task<ResponseData> SearchAsync(string keyword)
    {
        try
        {
            var products = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Where(p => (p.ProductName.Contains(keyword) || p.Description.Contains(keyword)) && p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var productDtos = _mapper.Map<List<ProductDTO>>(products);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Products retrieved successfully",
                productDtos
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

    public async Task<ResponseMessage> CreateAsync(int userId, ProductCreateDTO dto)
    {
        try
        {
            // Get seller by user ID
            var seller = await _db.Sellers.FirstOrDefaultAsync(s => s.UserId == userId);

            // ถ้าไม่มี Seller ให้สร้างอัตโนมัติสำหรับทดสอบ
            if (seller == null)
            {
                // ตรวจสอบว่ามี User หรือไม่
                var user = await _db.Users.FindAsync(userId);
                if (user == null)
                {
                    return new ResponseMessage(
                        System.Net.HttpStatusCode.NotFound,
                        false,
                        "User not found. Please create a user first."
                    );
                }

                // สร้าง Seller อัตโนมัติ
                seller = new Seller
                {
                    UserId = userId,
                    ShopName = $"ร้านของ {user.FirstName}",
                    ShopDescription = "ร้านค้าทั่วไป",
                    ShopImageUrl = null,
                    LogoUrl = null,
                    Description = "ร้านค้าทั่วไป",
                    PhoneNumber = user.PhoneNumber,
                    Address = null,
                    Rating = 0,
                    TotalSales = 0,
                    IsVerified = false,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _db.Sellers.AddAsync(seller);
                await _db.SaveChangesAsync();
            }

            // Check if category exists
            var category = await _db.Categories.FindAsync(dto.CategoryId);
            if (category == null)
            {
                return new ResponseMessage(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Category not found"
            );
            }

            // Create product
            var product = _mapper.Map<Product>(dto);
            product.SellerId = seller.SellerId;

            // Upload image if provided
            if (dto.Image != null)
            {
                var uploadResult = await _imageService.UploadImageAsync(dto.Image, "products");
                if (uploadResult.TaskStatus && uploadResult.Data != null)
                {
                    var imageData = uploadResult.Data as dynamic;
                    product.ImageUrl = imageData?.ImageUrl;
                }
            }
            product.Rating = 0;
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;

            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Product created successfully"
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

    public async Task<ResponseMessage> UpdateAsync(int id, int userId, ProductUpdateDTO dto)
    {
        try
        {
            var product = await _db.Products
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return new ResponseMessage(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Product not found"
            );
            }

            // Check ownership
            if (product.Seller.UserId != userId)
            {
                var user = await _db.Users.FindAsync(userId);
                if (user?.Role != "Admin")
                {
                    return new ResponseMessage(
                System.Net.HttpStatusCode.Forbidden,
                false,
                "You don't have permission to update this product"
            );
                }
            }

            // Check if category exists
            if (dto.CategoryId.HasValue)
            {
                var category = await _db.Categories.FindAsync(dto.CategoryId.Value);
                if (category == null)
                {
                    return new ResponseMessage(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Category not found"
            );
                }
            }

            // Upload new image if provided
            if (dto.Image != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    await _imageService.DeleteImageAsync(product.ImageUrl);
                }

                var uploadResult = await _imageService.UploadImageAsync(dto.Image, "products");
                if (uploadResult.TaskStatus && uploadResult.Data != null)
                {
                    var imageData = uploadResult.Data as dynamic;
                    product.ImageUrl = imageData?.ImageUrl;
                }
            }

            // Update fields
            if (dto.CategoryId.HasValue)
            {
                product.CategoryId = dto.CategoryId.Value;
            }
            product.ProductName = dto.ProductName ?? product.ProductName;
            product.Description = dto.Description ?? product.Description;
            if (dto.Price.HasValue)
            {
                product.Price = dto.Price.Value;
            }
            if (dto.Stock.HasValue)
            {
                product.Stock = dto.Stock.Value;
            }
            product.Unit = dto.Unit ?? product.Unit;
            if (dto.IsActive.HasValue)
            {
                product.IsActive = dto.IsActive.Value;
            }
            product.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Product updated successfully"
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

    public async Task<ResponseMessage> DeleteAsync(int id, int userId)
    {
        try
        {
            var product = await _db.Products
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return new ResponseMessage(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Product not found"
            );
            }

            // Check ownership or admin
            if (product.Seller.UserId != userId)
            {
                var user = await _db.Users.FindAsync(userId);
                if (user?.Role != "Admin")
                {
                    return new ResponseMessage(
                System.Net.HttpStatusCode.Forbidden,
                false,
                "You don't have permission to delete this product"
            );
                }
            }

            // Check if product has orders
            var hasOrders = await _db.OrderItems.AnyAsync(oi => oi.ProductId == id);
            if (hasOrders)
            {
                return new ResponseMessage(
                System.Net.HttpStatusCode.BadRequest,
                false,
                "Cannot delete product with existing orders"
            );
            }

            // Check if product is in any carts - remove them
            var cartItems = await _db.Carts.Where(c => c.ProductId == id).ToListAsync();
            if (cartItems.Any())
            {
                _db.Carts.RemoveRange(cartItems);
            }

            // Check if product has reviews - remove them
            var reviews = await _db.Reviews.Where(r => r.ProductId == id).ToListAsync();
            if (reviews.Any())
            {
                _db.Reviews.RemoveRange(reviews);
            }

            // Delete image if exists
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                await _imageService.DeleteImageAsync(product.ImageUrl);
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Product deleted successfully"
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

    public async Task<ResponseMessage> UpdateStockAsync(int productId, int quantity)
    {
        try
        {
            var product = await _db.Products.FindAsync(productId);
            if (product == null)
            {
                return new ResponseMessage(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Product not found"
            );
            }

            product.Stock -= quantity;
            product.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Stock updated successfully"
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

    public async Task<bool> RestoreStockAsync(int productId, int quantity)
    {
        try
        {
            var product = await _db.Products.FindAsync(productId);
            if (product == null) return false;

            product.Stock += quantity;
            product.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
