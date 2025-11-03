using Microsoft.EntityFrameworkCore;
using ClassLibrary.Models.Data;
using ClassLibrary.Models.Dto;
using ClassLibrary.Models.Dto.Cultures;
using Store.Data;

namespace Store.Services;

public class CultureService : Service<Culture>, IServices.ICultureService
{
    private readonly IServices.IImageService _imageService;

    public CultureService(ApplicationDbContext db, AutoMapper.IMapper mapper, IServices.IImageService imageService)
        : base(db, mapper)
    {
        _imageService = imageService;
    }

    public async Task<ResponseData> GetAllAsync()
    {
        try
        {
            var cultures = await _db.Cultures
                .Include(c => c.CultureImages)
                .OrderBy(c => c.CultureName)
                .ToListAsync();

            var cultureDtos = _mapper.Map<List<CultureDTO>>(cultures);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Cultures retrieved successfully",
                cultureDtos
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
            var culture = await _db.Cultures
                .Include(c => c.CultureImages)
                .FirstOrDefaultAsync(c => c.CultureId == id);

            if (culture == null)
            {
                return new ResponseData(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Culture not found",
                    null
                );
            }

            var cultureDto = _mapper.Map<CultureDTO>(culture);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Culture retrieved successfully",
                cultureDto
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

    public async Task<ResponseMessage> CreateAsync(CultureCreateDTO dto)
    {
        try
        {
            var existingCulture = await _db.Cultures
                .FirstOrDefaultAsync(c => c.CultureName == dto.CultureName);

            if (existingCulture != null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.BadRequest,
                    false,
                    "Culture with this name already exists"
                );
            }

            var culture = _mapper.Map<Culture>(dto);
            culture.CreatedAt = DateTime.Now;
            culture.UpdatedAt = DateTime.Now;
            culture.IsActive = true; // Set IsActive to true by default

            await _db.Cultures.AddAsync(culture);
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

            // Upload all images and create CultureImage records
            if (imagesToUpload.Any())
            {
                for (int i = 0; i < imagesToUpload.Count; i++)
                {
                    var uploadResult = await _imageService.UploadImageAsync(imagesToUpload[i], "cultures");
                    if (uploadResult.TaskStatus && uploadResult.Data != null)
                    {
                        var imageData = uploadResult.Data as dynamic;
                        var cultureImage = new CultureImage
                        {
                            CultureId = culture.CultureId,
                            ImageUrl = imageData?.ImageUrl,
                            DisplayOrder = i,
                            IsPrimary = i == 0, // First image is primary
                            CreatedAt = DateTime.Now
                        };

                        await _db.CultureImages.AddAsync(cultureImage);

                        // Set ImageUrl for backward compatibility (use first image)
                        if (i == 0)
                        {
                            culture.ImageUrl = imageData?.ImageUrl;
                        }
                    }
                }

                await _db.SaveChangesAsync();
            }

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Culture created successfully"
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

    public async Task<ResponseMessage> UpdateAsync(int id, CultureUpdateDTO dto)
    {
        try
        {
            var culture = await _db.Cultures.FindAsync(id);

            if (culture == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Culture not found"
                );
            }

            // Check if another culture with the same name exists
            if (dto.CultureName != null)
            {
                var existingCulture = await _db.Cultures
                    .FirstOrDefaultAsync(c => c.CultureName == dto.CultureName && c.CultureId != id);

                if (existingCulture != null)
                {
                    return new ResponseMessage(
                        System.Net.HttpStatusCode.BadRequest,
                        false,
                        "Another culture with this name already exists"
                    );
                }
            }

            // Handle image deletion
            if (dto.DeletedImageIds != null && dto.DeletedImageIds.Any())
            {
                var imagesToDelete = await _db.CultureImages
                    .Where(img => dto.DeletedImageIds.Contains(img.CultureImageId) && img.CultureId == id)
                    .ToListAsync();

                foreach (var img in imagesToDelete)
                {
                    // Delete from filesystem
                    await _imageService.DeleteImageAsync(img.ImageUrl);
                    // Delete from database
                    _db.CultureImages.Remove(img);
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
                var existingImages = await _db.CultureImages
                    .Where(img => img.CultureId == id)
                    .ToListAsync();

                var maxDisplayOrder = existingImages.Any()
                    ? existingImages.Max(img => img.DisplayOrder)
                    : -1;

                for (int i = 0; i < imagesToUpload.Count; i++)
                {
                    var uploadResult = await _imageService.UploadImageAsync(imagesToUpload[i], "cultures");
                    if (uploadResult.TaskStatus && uploadResult.Data != null)
                    {
                        var imageData = uploadResult.Data as dynamic;

                        // Check if this is the first image
                        var isFirstImage = !await _db.CultureImages.AnyAsync(img => img.CultureId == id);

                        var cultureImage = new CultureImage
                        {
                            CultureId = id,
                            ImageUrl = imageData?.ImageUrl,
                            DisplayOrder = maxDisplayOrder + 1 + i,
                            IsPrimary = isFirstImage, // Only first image is primary
                            CreatedAt = DateTime.Now
                        };

                        await _db.CultureImages.AddAsync(cultureImage);

                        // Update culture ImageUrl for backward compatibility (use first image)
                        if (isFirstImage)
                        {
                            culture.ImageUrl = imageData?.ImageUrl;
                        }
                    }
                }

                await _db.SaveChangesAsync();
            }

            // Update primary image URL if needed
            if (string.IsNullOrEmpty(culture.ImageUrl))
            {
                var primaryImage = await _db.CultureImages
                    .Where(img => img.CultureId == id)
                    .OrderBy(img => img.DisplayOrder)
                    .FirstOrDefaultAsync();

                if (primaryImage != null)
                {
                    culture.ImageUrl = primaryImage.ImageUrl;
                }
            }

            // Update fields
            if (dto.CultureName != null) culture.CultureName = dto.CultureName;
            if (dto.Description != null) culture.Description = dto.Description;
            if (dto.Category != null) culture.Category = dto.Category;
            if (dto.IsActive.HasValue) culture.IsActive = dto.IsActive.Value;
            culture.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Culture updated successfully"
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

    public async Task<ResponseMessage> DeleteAsync(int id)
    {
        try
        {
            var culture = await _db.Cultures
                .Include(c => c.CultureImages)
                .FirstOrDefaultAsync(c => c.CultureId == id);

            if (culture == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Culture not found"
                );
            }

            // Delete all culture images from filesystem
            if (culture.CultureImages != null && culture.CultureImages.Any())
            {
                foreach (var img in culture.CultureImages)
                {
                    await _imageService.DeleteImageAsync(img.ImageUrl);
                }
            }

            // Delete culture (cascade will delete CultureImages from database)
            _db.Cultures.Remove(culture);
            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Culture deleted successfully"
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

    public async Task<ResponseData> GetActiveCulturesAsync()
    {
        try
        {
            var cultures = await _db.Cultures
                .Include(c => c.CultureImages)
                .Where(c => c.IsActive)
                .OrderBy(c => c.CultureName)
                .ToListAsync();

            var cultureDtos = _mapper.Map<List<CultureDTO>>(cultures);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Active cultures retrieved successfully",
                cultureDtos
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

    public async Task<ResponseMessage> ToggleActiveStatusAsync(int id)
    {
        try
        {
            var culture = await _db.Cultures.FindAsync(id);

            if (culture == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Culture not found"
                );
            }

            // Toggle status
            culture.IsActive = !culture.IsActive;
            culture.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                $"Culture {(culture.IsActive ? "enabled" : "disabled")} successfully"
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
