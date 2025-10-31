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

    public async Task<ResponseData> GetAllAsync(int pageNumber, int pageSize, string? search, int? categoryId, int? sellerId, bool? isActive = null)
    {
        try
        {
            var query = _db.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.ProductImages)
                .AsQueryable();

            // Apply IsActive filter
            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }
            // If isActive is not specified, show all products (no filter)

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
                .Include(p => p.ProductImages)
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
                .Include(p => p.ProductImages)
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
                .Include(p => p.ProductImages)
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
                .Include(p => p.ProductImages)
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
            product.Rating = 0;
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;

            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();

            // Upload images
            var imagesToUpload = new List<IFormFile>();

            // Support both single image (backward compatibility) and multiple images
            if (dto.Image != null)
            {
                imagesToUpload.Add(dto.Image);
            }

            if (dto.Images != null && dto.Images.Any())
            {
                imagesToUpload.AddRange(dto.Images);
            }

            // Upload all images and create ProductImage records
            if (imagesToUpload.Any())
            {
                for (int i = 0; i < imagesToUpload.Count; i++)
                {
                    var uploadResult = await _imageService.UploadImageAsync(imagesToUpload[i], "products");
                    if (uploadResult.TaskStatus && uploadResult.Data != null)
                    {
                        var imageData = uploadResult.Data as dynamic;
                        var productImage = new ProductImage
                        {
                            ProductId = product.ProductId,
                            ImageUrl = imageData?.ImageUrl,
                            DisplayOrder = i,
                            IsPrimary = i == 0, // First image is primary
                            CreatedAt = DateTime.Now
                        };

                        await _db.ProductImages.AddAsync(productImage);

                        // Set ImageUrl for backward compatibility (use first image)
                        if (i == 0)
                        {
                            product.ImageUrl = imageData?.ImageUrl;
                        }
                    }
                }

                await _db.SaveChangesAsync();
            }

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

            // Handle image deletion
            if (dto.DeleteImageIds != null && dto.DeleteImageIds.Any())
            {
                var imagesToDelete = await _db.ProductImages
                    .Where(img => dto.DeleteImageIds.Contains(img.ProductImageId) && img.ProductId == id)
                    .ToListAsync();

                foreach (var img in imagesToDelete)
                {
                    // Delete from filesystem
                    await _imageService.DeleteImageAsync(img.ImageUrl);
                    // Delete from database
                    _db.ProductImages.Remove(img);
                }

                await _db.SaveChangesAsync();
            }

            // Handle new image uploads
            var imagesToUpload = new List<IFormFile>();

            // Support both single image (backward compatibility) and multiple images
            if (dto.Image != null)
            {
                imagesToUpload.Add(dto.Image);
            }

            if (dto.Images != null && dto.Images.Any())
            {
                imagesToUpload.AddRange(dto.Images);
            }

            // Upload new images
            if (imagesToUpload.Any())
            {
                // Get current max display order
                var existingImages = await _db.ProductImages
                    .Where(img => img.ProductId == id)
                    .ToListAsync();

                var maxDisplayOrder = existingImages.Any()
                    ? existingImages.Max(img => img.DisplayOrder)
                    : -1;

                for (int i = 0; i < imagesToUpload.Count; i++)
                {
                    var uploadResult = await _imageService.UploadImageAsync(imagesToUpload[i], "products");
                    if (uploadResult.TaskStatus && uploadResult.Data != null)
                    {
                        var imageData = uploadResult.Data as dynamic;

                        // Check if this is the first image
                        var isFirstImage = !await _db.ProductImages.AnyAsync(img => img.ProductId == id);

                        var productImage = new ProductImage
                        {
                            ProductId = id,
                            ImageUrl = imageData?.ImageUrl,
                            DisplayOrder = maxDisplayOrder + 1 + i,
                            IsPrimary = isFirstImage, // Only first image is primary
                            CreatedAt = DateTime.Now
                        };

                        await _db.ProductImages.AddAsync(productImage);

                        // Update product ImageUrl for backward compatibility (use first image)
                        if (isFirstImage)
                        {
                            product.ImageUrl = imageData?.ImageUrl;
                        }
                    }
                }

                await _db.SaveChangesAsync();
            }

            // Update primary image URL if needed
            if (string.IsNullOrEmpty(product.ImageUrl))
            {
                var primaryImage = await _db.ProductImages
                    .Where(img => img.ProductId == id)
                    .OrderBy(img => img.DisplayOrder)
                    .FirstOrDefaultAsync();

                if (primaryImage != null)
                {
                    product.ImageUrl = primaryImage.ImageUrl;
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

    public async Task<ResponseMessage> DeleteAsync(int id, int userId, bool hardDelete = false)
    {
        try
        {
            var product = await _db.Products
                .Include(p => p.Seller)
                .Include(p => p.OrderItems)
                .Include(p => p.ProductImages)
                .Include(p => p.Carts)
                .Include(p => p.Reviews)
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

            // Check if product is used in orders
            bool hasOrders = product.OrderItems.Any();

            if (hardDelete)
            {
                // Hard delete: Actually remove from database
                // But only if no orders reference this product
                if (hasOrders)
                {
                    return new ResponseMessage(
                        System.Net.HttpStatusCode.BadRequest,
                        false,
                        "Cannot permanently delete product that has been ordered. Use soft delete instead."
                    );
                }

                // Delete related data first
                if (product.ProductImages.Any())
                {
                    foreach (var image in product.ProductImages.ToList())
                    {
                        await _imageService.DeleteImageAsync(image.ImageUrl);
                        _db.ProductImages.Remove(image);
                    }
                }

                if (product.Carts.Any())
                {
                    _db.Carts.RemoveRange(product.Carts);
                }

                if (product.Reviews.Any())
                {
                    _db.Reviews.RemoveRange(product.Reviews);
                }

                // Delete main image if exists
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    await _imageService.DeleteImageAsync(product.ImageUrl);
                }

                // Delete the product
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();

                return new ResponseMessage(
                    System.Net.HttpStatusCode.OK,
                    true,
                    "Product permanently deleted successfully"
                );
            }
            else
            {
                // Soft delete: Set IsActive to false
                product.IsActive = false;
                product.UpdatedAt = DateTime.Now;

                await _db.SaveChangesAsync();

                return new ResponseMessage(
                    System.Net.HttpStatusCode.OK,
                    true,
                    hasOrders
                        ? "Product disabled successfully (has order history)"
                        : "Product disabled successfully"
                );
            }
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

    public async Task<ResponseMessage> ToggleActiveStatusAsync(int productId, int userId)
    {
        try
        {
            var product = await _db.Products
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

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
                        "You don't have permission to modify this product"
                    );
                }
            }

            // Toggle IsActive status
            product.IsActive = !product.IsActive;
            product.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                $"Product {(product.IsActive ? "enabled" : "disabled")} successfully"
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

    public async Task<ResponseData> CheckProductUsageAsync(int productId)
    {
        try
        {
            var product = await _db.Products
                .Include(p => p.OrderItems)
                .Include(p => p.Carts)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return new ResponseData(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Product not found",
                    null
                );
            }

            var usageInfo = new
            {
                ProductId = productId,
                HasOrders = product.OrderItems.Any(),
                OrderCount = product.OrderItems.Count,
                InCartsCount = product.Carts.Count,
                ReviewCount = product.Reviews.Count,
                CanHardDelete = !product.OrderItems.Any(),
                RecommendedAction = product.OrderItems.Any()
                    ? "Soft delete recommended - product has order history"
                    : "Can be permanently deleted"
            };

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Product usage retrieved successfully",
                usageInfo
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
}
