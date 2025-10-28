using Microsoft.EntityFrameworkCore;
using ClassLibrary.Models.Data;
using ClassLibrary.Models.Dto;
using Store.Data;
using Microsoft.AspNetCore.Hosting;

namespace Store.Services;

public class SellerService : Service<Seller>, IServices.ISellerService
{
    private readonly IServices.IImageService _imageService;
    private readonly IWebHostEnvironment _env;

    public SellerService(ApplicationDbContext db, AutoMapper.IMapper mapper, IServices.IImageService imageService, IWebHostEnvironment env)
        : base(db, mapper)
    {
        _imageService = imageService;
        _env = env;
    }

    public async Task<ResponseData> GetAllAsync()
    {
        try
        {
            var sellers = await _db.Sellers
                .Include(s => s.User)
                .Where(s => s.IsActive)
                .ToListAsync();

            var sellerDtos = _mapper.Map<List<SellerDTO>>(sellers);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Sellers retrieved successfully",
                sellerDtos
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
            var seller = await _db.Sellers
                .Include(s => s.User)
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.SellerId == id);

            if (seller == null)
            {
                return new ResponseData(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Seller not found",
                null
            );
            }

            var sellerDto = _mapper.Map<SellerDetailDTO>(seller);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Seller retrieved successfully",
                sellerDto
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

    public async Task<ResponseMessage> RegisterSellerAsync(int userId, SellerCreateDTO dto)
    {
        try
        {
            // Check if user exists
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                return new ResponseMessage(
                System.Net.HttpStatusCode.NotFound,
                false,
                "User not found"
            );
            }

            // Check if user is already a seller
            var existingSeller = await _db.Sellers.FirstOrDefaultAsync(s => s.UserId == userId);
            if (existingSeller != null)
            {
                return new ResponseMessage(
                System.Net.HttpStatusCode.BadRequest,
                false,
                "User is already registered as a seller"
            );
            }

            // Check if shop name already exists
            var existingShopName = await _db.Sellers
                .FirstOrDefaultAsync(s => s.ShopName == dto.ShopName);
            if (existingShopName != null)
            {
                return new ResponseMessage(
                System.Net.HttpStatusCode.BadRequest,
                false,
                "Shop name already exists"
            );
            }

            // Create seller
            var seller = _mapper.Map<Seller>(dto);
            seller.UserId = userId;
            seller.Rating = 0;
            seller.IsActive = true;
            seller.CreatedAt = DateTime.Now;
            seller.UpdatedAt = DateTime.Now;

            await _db.Sellers.AddAsync(seller);

            // Update user role to Seller
            user.Role = "Seller";
            user.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Seller registered successfully"
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

    public async Task<ResponseMessage> UpdateProfileAsync(int sellerId, SellerUpdateDTO dto)
    {
        try
        {
            var seller = await _db.Sellers.FirstOrDefaultAsync(s => s.SellerId == sellerId);

            if (seller == null)
            {
                return new ResponseMessage(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Seller not found"
            );
            }

            // Check if shop name already exists (for another seller)
            if (dto.ShopName != null)
            {
                var existingShopName = await _db.Sellers
                    .FirstOrDefaultAsync(s => s.ShopName == dto.ShopName && s.SellerId != sellerId);
                if (existingShopName != null)
                {
                    return new ResponseMessage(
                System.Net.HttpStatusCode.BadRequest,
                false,
                "Shop name already exists"
            );
                }
            }

            // Update fields
            if (!string.IsNullOrEmpty(dto.ShopName))
                seller.ShopName = dto.ShopName;
            if (!string.IsNullOrEmpty(dto.ShopDescription))
                seller.ShopDescription = dto.ShopDescription;
            if (!string.IsNullOrEmpty(dto.PhoneNumber))
                seller.PhoneNumber = dto.PhoneNumber;
            if (!string.IsNullOrEmpty(dto.Address))
                seller.Address = dto.Address;

            // Extract path from URL if needed (remove domain if present)
            if (!string.IsNullOrEmpty(dto.ShopImageUrl))
                seller.ShopImageUrl = ExtractImagePath(dto.ShopImageUrl);
            if (!string.IsNullOrEmpty(dto.LogoUrl))
                seller.LogoUrl = ExtractImagePath(dto.LogoUrl);
            if (!string.IsNullOrEmpty(dto.QrCodeUrl))
                seller.QrCodeUrl = ExtractImagePath(dto.QrCodeUrl);

            seller.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Seller updated successfully"
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

    public async Task<ResponseData> GetByUserIdAsync(int userId)
    {
        try
        {
            var seller = await _db.Sellers
                .Include(s => s.User)
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (seller == null)
            {
                return new ResponseData(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Seller not found for this user",
                null
            );
            }

            var sellerDto = _mapper.Map<SellerDetailDTO>(seller);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Seller retrieved successfully",
                sellerDto
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

    public async Task<ResponseData> GetProfileAsync(int sellerId)
    {
        try
        {
            var seller = await _db.Sellers
                .Include(s => s.User)
                .Include(s => s.Products)
                    .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(s => s.SellerId == sellerId);

            if (seller == null)
            {
                return new ResponseData(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Seller not found",
                null
            );
            }

            var sellerDto = _mapper.Map<SellerDetailDTO>(seller);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Seller profile retrieved successfully",
                sellerDto
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

    public async Task<ResponseData> GetProductsAsync(int sellerId)
    {
        try
        {
            var seller = await _db.Sellers.FindAsync(sellerId);
            if (seller == null)
            {
                return new ResponseData(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Seller not found",
                null
            );
            }

            var products = await _db.Products
                .Include(p => p.Category)
                .Where(p => p.SellerId == sellerId && p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var productDtos = _mapper.Map<List<ProductDTO>>(products);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Seller products retrieved successfully",
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

    public async Task<ResponseData> DeleteAsync(int id, int userId)
    {
        try
        {
            var seller = await _db.Sellers.FindAsync(id);

            if (seller == null)
            {
                return new ResponseData(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Seller not found",
                null
            );
            }

            // Check ownership or admin
            if (seller.UserId != userId)
            {
                var user = await _db.Users.FindAsync(userId);
                if (user?.Role != "Admin")
                {
                    return new ResponseData(
                System.Net.HttpStatusCode.Forbidden,
                false,
                "You don't have permission to delete this seller",
                null
            );
                }
            }

            // Check if seller has products
            var hasProducts = await _db.Products.AnyAsync(p => p.SellerId == id);
            if (hasProducts)
            {
                return new ResponseData(
                System.Net.HttpStatusCode.BadRequest,
                false,
                "Cannot delete seller with existing products",
                null
            );
            }

            _db.Sellers.Remove(seller);
            await _db.SaveChangesAsync();

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Seller deleted successfully",
                null
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

    public async Task<ResponseMessage> UpdateRatingAsync(int sellerId)
    {
        try
        {
            // Calculate average rating from all seller's products
            var products = await _db.Products
                .Where(p => p.SellerId == sellerId)
                .ToListAsync();

            var seller = await _db.Sellers.FindAsync(sellerId);
            if (seller == null)
            {
                return new ResponseMessage(
                System.Net.HttpStatusCode.NotFound,
                false,
                "Seller not found"
            );
            }

            if (products.Any())
            {
                var totalRating = products.Sum(p => p.Rating);
                var averageRating = totalRating / products.Count;

                seller.Rating = (decimal)averageRating;
                seller.UpdatedAt = DateTime.Now;
                await _db.SaveChangesAsync();
            }

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Seller rating updated successfully"
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

    public async Task<ResponseData> UploadShopImageAsync(int userId, Microsoft.AspNetCore.Http.IFormFile file)
    {
        try
        {
            Console.WriteLine($"[DEBUG] UploadShopImageAsync - userId: {userId}, fileName: {file.FileName}");

            // Find seller by userId
            var seller = await _db.Sellers
                .Where(s => s.UserId == userId && s.IsActive)
                .FirstOrDefaultAsync();

            if (seller == null)
            {
                Console.WriteLine($"[DEBUG] Seller not found for userId: {userId}");
                return new ResponseData(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Seller not found for this user",
                    null
                );
            }

            Console.WriteLine($"[DEBUG] Found seller: SellerId={seller.SellerId}, OldShopImageUrl={seller.ShopImageUrl}");

            // Create shop-images directory if it doesn't exist
            var shopImagesPath = Path.Combine(_env.WebRootPath, "images", "shop-images");
            if (!Directory.Exists(shopImagesPath))
            {
                Directory.CreateDirectory(shopImagesPath);
                Console.WriteLine($"[DEBUG] Created directory: {shopImagesPath}");
            }

            // Delete old shop image if exists
            if (!string.IsNullOrEmpty(seller.ShopImageUrl))
            {
                var oldFilePath = Path.Combine(_env.WebRootPath, seller.ShopImageUrl.TrimStart('/'));
                Console.WriteLine($"[DEBUG] Attempting to delete old file: {oldFilePath}");
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                    Console.WriteLine($"[DEBUG] Deleted old shop image file");
                }
                else
                {
                    Console.WriteLine($"[DEBUG] Old file does not exist");
                }
            }

            // Generate unique filename
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"seller-{seller.SellerId}-shop-{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(shopImagesPath, fileName);

            Console.WriteLine($"[DEBUG] New file path: {filePath}");

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            Console.WriteLine($"[DEBUG] File saved successfully");

            // Update seller's ShopImageUrl
            var shopImageUrl = $"/images/shop-images/{fileName}";
            seller.ShopImageUrl = shopImageUrl;
            seller.UpdatedAt = DateTime.Now;

            _db.Sellers.Update(seller);
            await _db.SaveChangesAsync();

            Console.WriteLine($"[DEBUG] Database updated, new ShopImageUrl: {shopImageUrl}");

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Shop image uploaded successfully",
                shopImageUrl
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] UploadShopImageAsync failed: {ex.Message}");
            Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
            return new ResponseData(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}",
                null
            );
        }
    }

    public async Task<ResponseData> UploadLogoAsync(int userId, Microsoft.AspNetCore.Http.IFormFile file)
    {
        try
        {
            Console.WriteLine($"[DEBUG] UploadLogoAsync - userId: {userId}, fileName: {file.FileName}");

            // Find seller by userId
            var seller = await _db.Sellers
                .Where(s => s.UserId == userId && s.IsActive)
                .FirstOrDefaultAsync();

            if (seller == null)
            {
                Console.WriteLine($"[DEBUG] Seller not found for userId: {userId}");
                return new ResponseData(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Seller not found for this user",
                    null
                );
            }

            Console.WriteLine($"[DEBUG] Found seller: SellerId={seller.SellerId}, OldLogoUrl={seller.LogoUrl}");

            // Create logos directory if it doesn't exist
            var logosPath = Path.Combine(_env.WebRootPath, "images", "logos");
            if (!Directory.Exists(logosPath))
            {
                Directory.CreateDirectory(logosPath);
                Console.WriteLine($"[DEBUG] Created directory: {logosPath}");
            }

            // Delete old logo if exists
            if (!string.IsNullOrEmpty(seller.LogoUrl))
            {
                var oldFilePath = Path.Combine(_env.WebRootPath, seller.LogoUrl.TrimStart('/'));
                Console.WriteLine($"[DEBUG] Attempting to delete old file: {oldFilePath}");
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                    Console.WriteLine($"[DEBUG] Deleted old logo file");
                }
                else
                {
                    Console.WriteLine($"[DEBUG] Old file does not exist");
                }
            }

            // Generate unique filename
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"seller-{seller.SellerId}-logo-{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(logosPath, fileName);

            Console.WriteLine($"[DEBUG] New file path: {filePath}");

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            Console.WriteLine($"[DEBUG] File saved successfully");

            // Update seller's LogoUrl
            var logoUrl = $"/images/logos/{fileName}";
            seller.LogoUrl = logoUrl;
            seller.UpdatedAt = DateTime.Now;

            _db.Sellers.Update(seller);
            await _db.SaveChangesAsync();

            Console.WriteLine($"[DEBUG] Database updated, new LogoUrl: {logoUrl}");

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Logo uploaded successfully",
                logoUrl
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] UploadLogoAsync failed: {ex.Message}");
            Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
            return new ResponseData(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}",
                null
            );
        }
    }

    public async Task<ResponseData> UploadQRCodeAsync(int userId, Microsoft.AspNetCore.Http.IFormFile file)
    {
        try
        {
            Console.WriteLine($"[DEBUG] UploadQRCodeAsync - userId: {userId}, fileName: {file.FileName}");

            // Find seller by userId
            var seller = await _db.Sellers
                .Where(s => s.UserId == userId && s.IsActive)
                .FirstOrDefaultAsync();

            if (seller == null)
            {
                Console.WriteLine($"[DEBUG] Seller not found for userId: {userId}");
                return new ResponseData(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Seller not found for this user",
                    null
                );
            }

            Console.WriteLine($"[DEBUG] Found seller: SellerId={seller.SellerId}, OldQrCodeUrl={seller.QrCodeUrl}");

            // Create qrcodes directory if it doesn't exist
            var qrcodesPath = Path.Combine(_env.WebRootPath, "images", "qrcodes");
            if (!Directory.Exists(qrcodesPath))
            {
                Directory.CreateDirectory(qrcodesPath);
                Console.WriteLine($"[DEBUG] Created directory: {qrcodesPath}");
            }

            // Delete old QR code if exists
            if (!string.IsNullOrEmpty(seller.QrCodeUrl))
            {
                var oldFilePath = Path.Combine(_env.WebRootPath, seller.QrCodeUrl.TrimStart('/'));
                Console.WriteLine($"[DEBUG] Attempting to delete old file: {oldFilePath}");
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                    Console.WriteLine($"[DEBUG] Deleted old QR code file");
                }
                else
                {
                    Console.WriteLine($"[DEBUG] Old file does not exist");
                }
            }

            // Generate unique filename
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"seller-{seller.SellerId}-qrcode-{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(qrcodesPath, fileName);

            Console.WriteLine($"[DEBUG] New file path: {filePath}");

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            Console.WriteLine($"[DEBUG] File saved successfully");

            // Update seller's QrCodeUrl
            var qrCodeUrl = $"/images/qrcodes/{fileName}";
            seller.QrCodeUrl = qrCodeUrl;
            seller.UpdatedAt = DateTime.Now;

            _db.Sellers.Update(seller);
            await _db.SaveChangesAsync();

            Console.WriteLine($"[DEBUG] Database updated, new QrCodeUrl: {qrCodeUrl}");

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "QR Code uploaded successfully",
                qrCodeUrl
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] UploadQRCodeAsync failed: {ex.Message}");
            Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
            return new ResponseData(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}",
                null
            );
        }
    }

    /// <summary>
    /// Extract relative path from URL (remove domain if present)
    /// Example: "https://example.com/images/test.jpg" => "/images/test.jpg"
    /// Example: "/images/test.jpg" => "/images/test.jpg" (unchanged)
    /// </summary>
    private string ExtractImagePath(string url)
    {
        if (string.IsNullOrEmpty(url))
            return url;

        // If URL starts with http:// or https://, extract the path part
        if (url.StartsWith("http://") || url.StartsWith("https://"))
        {
            try
            {
                var uri = new Uri(url);
                return uri.AbsolutePath; // Returns "/images/..." without domain
            }
            catch
            {
                // If URI parsing fails, try to find /images/ and extract from there
                int imagesIndex = url.IndexOf("/images/");
                if (imagesIndex >= 0)
                {
                    return url.Substring(imagesIndex);
                }
                return url; // Return as-is if we can't extract
            }
        }

        // Already a relative path, return as-is
        return url;
    }
}
