using System.Net;
using AutoMapper;
using ClassLibrary.Models.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Store.Services;

public class LocationService : ILocationService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;

    public LocationService(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment env)
    {
        _context = context;
        _mapper = mapper;
        _env = env;
    }

    public async Task<ResponseData> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null)
    {
        try
        {
            var query = _context.Locations
                .Include(x => x.Seller)
                .Where(x => x.IsActive)
                .AsQueryable();

            // Search functionality
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x =>
                    x.LocationName.Contains(search) ||
                    (x.Description != null && x.Description.Contains(search)) ||
                    (x.Address != null && x.Address.Contains(search))
                );
            }

            // Get total count for pagination
            var total = await query.CountAsync();

            // Apply pagination
            var locations = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var locationDTOs = _mapper.Map<List<LocationDTO>>(locations);

            return new ResponseData(
                HttpStatusCode.OK,
                true,
                $"พบข้อมูล {total} รายการ (แสดงหน้า {pageNumber} จาก {Math.Ceiling((double)total / pageSize)} หน้า)",
                locationDTOs
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                HttpStatusCode.InternalServerError,
                false,
                $"เกิดข้อผิดพลาด: {ex.Message}"
            );
        }
    }

    public async Task<ResponseData> GetByIdAsync(int id)
    {
        try
        {
            var location = await _context.Locations
                .Include(x => x.Seller)
                .Include(x => x.Products)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (location == null)
            {
                return new ResponseData(
                    HttpStatusCode.NotFound,
                    false,
                    "ไม่พบข้อมูล Location"
                );
            }

            var locationDTO = _mapper.Map<LocationDetailDTO>(location);

            return new ResponseData(
                HttpStatusCode.OK,
                true,
                "ดึงข้อมูล Location สำเร็จ",
                locationDTO
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                HttpStatusCode.InternalServerError,
                false,
                $"เกิดข้อผิดพลาด: {ex.Message}"
            );
        }
    }

    public async Task<ResponseData> GetByLocationIdAsync(string locationId)
    {
        try
        {
            var location = await _context.Locations
                .Include(x => x.Seller)
                .FirstOrDefaultAsync(x => x.LocationId == locationId);

            if (location == null)
            {
                return new ResponseData(
                    HttpStatusCode.NotFound,
                    false,
                    "ไม่พบข้อมูล Location"
                );
            }

            var locationDTO = _mapper.Map<LocationDTO>(location);

            return new ResponseData(
                HttpStatusCode.OK,
                true,
                "ดึงข้อมูล Location สำเร็จ",
                locationDTO
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                HttpStatusCode.InternalServerError,
                false,
                $"เกิดข้อผิดพลาด: {ex.Message}"
            );
        }
    }

    public async Task<ResponseData> CreateAsync(LocationCreateDTO dto)
    {
        try
        {
            // Validate seller exists if provided
            if (dto.SellerId.HasValue)
            {
                var sellerExists = await _context.Sellers.AnyAsync(s => s.SellerId == dto.SellerId.Value);
                if (!sellerExists)
                {
                    return new ResponseData(
                        HttpStatusCode.BadRequest,
                        false,
                        "ไม่พบข้อมูล Seller ที่ระบุ"
                    );
                }
            }

            // Generate LocationId
            var locationId = await GenerateLocationIdAsync();

            // Map DTO to entity
            var location = _mapper.Map<Location>(dto);
            location.LocationId = locationId;
            location.CreatedAt = DateTime.Now;
            location.UpdatedAt = DateTime.Now;

            // Add to database
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            return new ResponseData(
                HttpStatusCode.OK,
                true,
                $"สร้าง Location สำเร็จ (LocationId: {locationId})",
                location.Id  // Return the location ID so frontend can upload image
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                HttpStatusCode.InternalServerError,
                false,
                $"เกิดข้อผิดพลาด: {ex.Message}"
            );
        }
    }

    public async Task<ResponseMessage> UpdateAsync(string refId, LocationUpdateDTO dto)
    {
        try
        {
            // Find location by RefId or LocationId
            var location = await _context.Locations
                .FirstOrDefaultAsync(x => x.RefId == refId || x.LocationId == refId);

            if (location == null)
            {
                return new ResponseMessage(
                    HttpStatusCode.NotFound,
                    false,
                    "ไม่พบข้อมูล Location"
                );
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(dto.RefId))
                location.RefId = dto.RefId;

            if (!string.IsNullOrEmpty(dto.LocationName))
                location.LocationName = dto.LocationName;

            if (!string.IsNullOrEmpty(dto.Description))
                location.Description = dto.Description;

            if (!string.IsNullOrEmpty(dto.LocationType))
                location.LocationType = dto.LocationType;

            if (dto.Latitude.HasValue)
                location.Latitude = dto.Latitude.Value;

            if (dto.Longitude.HasValue)
                location.Longitude = dto.Longitude.Value;

            if (!string.IsNullOrEmpty(dto.Address))
                location.Address = dto.Address;

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
                location.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrEmpty(dto.ImageUrl))
                location.ImageUrl = dto.ImageUrl;

            if (!string.IsNullOrEmpty(dto.IconUrl))
                location.IconUrl = dto.IconUrl;

            if (!string.IsNullOrEmpty(dto.IconColor))
                location.IconColor = dto.IconColor;

            if (dto.IsActive.HasValue)
                location.IsActive = dto.IsActive.Value;

            location.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ResponseMessage(
                HttpStatusCode.OK,
                true,
                "อัปเดต Location สำเร็จ"
            );
        }
        catch (Exception ex)
        {
            return new ResponseMessage(
                HttpStatusCode.InternalServerError,
                false,
                $"เกิดข้อผิดพลาด: {ex.Message}"
            );
        }
    }

    public async Task<ResponseMessage> DeleteAsync(int id)
    {
        try
        {
            var location = await _context.Locations.FindAsync(id);

            if (location == null)
            {
                return new ResponseMessage(
                    HttpStatusCode.NotFound,
                    false,
                    "ไม่พบข้อมูล Location"
                );
            }

            // Hard delete - remove from database permanently
            // Delete associated image file if exists
            if (!string.IsNullOrEmpty(location.ImageUrl))
            {
                try
                {
                    var imagePath = Path.Combine(_env.WebRootPath, location.ImageUrl.TrimStart('/'));
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                        Console.WriteLine($"[DEBUG] Deleted location image: {imagePath}");
                    }
                }
                catch (Exception imgEx)
                {
                    Console.WriteLine($"[WARNING] Could not delete image file: {imgEx.Message}");
                    // Continue with deletion even if image deletion fails
                }
            }

            // Remove location from database
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            return new ResponseMessage(
                HttpStatusCode.OK,
                true,
                "ลบ Location สำเร็จ"
            );
        }
        catch (Exception ex)
        {
            return new ResponseMessage(
                HttpStatusCode.InternalServerError,
                false,
                $"เกิดข้อผิดพลาด: {ex.Message}"
            );
        }
    }

    public async Task<ResponseData> GetByTypeAsync(string locationType)
    {
        try
        {
            var locations = await _context.Locations
                .Include(x => x.Seller)
                .Where(x => x.IsActive && x.LocationType == locationType)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var locationDTOs = _mapper.Map<List<LocationDTO>>(locations);

            return new ResponseData(
                HttpStatusCode.OK,
                true,
                $"พบ {locationDTOs.Count} รายการประเภท {locationType}",
                locationDTOs
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                HttpStatusCode.InternalServerError,
                false,
                $"เกิดข้อผิดพลาด: {ex.Message}"
            );
        }
    }

    public async Task<ResponseData> GetBySellerIdAsync(int sellerId)
    {
        try
        {
            var locations = await _context.Locations
                .Include(x => x.Seller)
                .Where(x => x.IsActive && x.SellerId == sellerId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var locationDTOs = _mapper.Map<List<LocationDTO>>(locations);

            return new ResponseData(
                HttpStatusCode.OK,
                true,
                $"พบ {locationDTOs.Count} รายการของ Seller ID {sellerId}",
                locationDTOs
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                HttpStatusCode.InternalServerError,
                false,
                $"เกิดข้อผิดพลาด: {ex.Message}"
            );
        }
    }

    public async Task<ResponseData> GetNearbyLocationsAsync(decimal latitude, decimal longitude, decimal radiusKm)
    {
        try
        {
            var locations = await _context.Locations
                .Include(x => x.Seller)
                .Where(x => x.IsActive)
                .ToListAsync();

            // Filter locations within radius using Haversine formula
            var nearbyLocations = locations
                .Where(loc =>
                {
                    var distance = CalculateDistance(
                        (double)latitude,
                        (double)longitude,
                        (double)loc.Latitude,
                        (double)loc.Longitude
                    );
                    return distance <= (double)radiusKm;
                })
                .OrderBy(loc => CalculateDistance(
                    (double)latitude,
                    (double)longitude,
                    (double)loc.Latitude,
                    (double)loc.Longitude
                ))
                .ToList();

            var locationDTOs = _mapper.Map<List<LocationDTO>>(nearbyLocations);

            return new ResponseData(
                HttpStatusCode.OK,
                true,
                $"พบ {locationDTOs.Count} รายการในรัศมี {radiusKm} กม.",
                locationDTOs
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                HttpStatusCode.InternalServerError,
                false,
                $"เกิดข้อผิดพลาด: {ex.Message}"
            );
        }
    }

    public async Task<ResponseData> UploadLocationImageAsync(int locationId, IFormFile file)
    {
        try
        {
            Console.WriteLine($"[DEBUG] UploadLocationImageAsync - locationId: {locationId}, fileName: {file.FileName}");

            // Find location by ID
            var location = await _context.Locations
                .FirstOrDefaultAsync(l => l.Id == locationId && l.IsActive);

            if (location == null)
            {
                Console.WriteLine($"[DEBUG] Location not found for locationId: {locationId}");
                return new ResponseData(
                    HttpStatusCode.NotFound,
                    false,
                    "ไม่พบข้อมูล Location",
                    null
                );
            }

            Console.WriteLine($"[DEBUG] Found location: Id={location.Id}, OldImageUrl={location.ImageUrl}");

            // Create location-images directory if it doesn't exist
            var locationImagesPath = Path.Combine(_env.WebRootPath, "images", "location-images");
            if (!Directory.Exists(locationImagesPath))
            {
                Directory.CreateDirectory(locationImagesPath);
                Console.WriteLine($"[DEBUG] Created directory: {locationImagesPath}");
            }

            // Delete old location image if exists
            if (!string.IsNullOrEmpty(location.ImageUrl))
            {
                var oldFilePath = Path.Combine(_env.WebRootPath, location.ImageUrl.TrimStart('/'));
                Console.WriteLine($"[DEBUG] Attempting to delete old file: {oldFilePath}");
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                    Console.WriteLine($"[DEBUG] Deleted old location image file");
                }
                else
                {
                    Console.WriteLine($"[DEBUG] Old file does not exist");
                }
            }

            // Generate unique filename
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"location-{location.Id}-{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(locationImagesPath, fileName);

            Console.WriteLine($"[DEBUG] New file path: {filePath}");

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            Console.WriteLine($"[DEBUG] File saved successfully");

            // Update location's ImageUrl
            var imageUrl = $"/images/location-images/{fileName}";
            location.ImageUrl = imageUrl;
            location.UpdatedAt = DateTime.Now;

            _context.Locations.Update(location);
            await _context.SaveChangesAsync();

            Console.WriteLine($"[DEBUG] Database updated, new ImageUrl: {imageUrl}");

            return new ResponseData(
                HttpStatusCode.OK,
                true,
                "อัปโหลดรูปภาพสำเร็จ",
                imageUrl
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] UploadLocationImageAsync failed: {ex.Message}");
            Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
            return new ResponseData(
                HttpStatusCode.InternalServerError,
                false,
                $"เกิดข้อผิดพลาด: {ex.Message}",
                null
            );
        }
    }

    public async Task<string> GenerateLocationIdAsync()
    {
        var lastLocation = await _context.Locations
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();

        var nextNumber = lastLocation != null ? lastLocation.Id + 1 : 1;
        return $"LO-{nextNumber:D4}"; // Format: LO-0001, LO-0002, etc.
    }

    // Haversine formula - Calculate distance between two GPS coordinates
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in kilometers

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c; // Distance in kilometers
    }

    private double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}
