using Microsoft.EntityFrameworkCore;
using ClassLibrary.Models.Data;
using ClassLibrary.Models.Dto;
using Store.Data;

namespace Store.Services;

public class AppBannerService : Service<AppBanner>, IServices.IAppBannerService
{
    private readonly IServices.IImageService _imageService;

    public AppBannerService(ApplicationDbContext db, AutoMapper.IMapper mapper, IServices.IImageService imageService)
        : base(db, mapper)
    {
        _imageService = imageService;
    }

    public async Task<ResponseData> GetAllAsync()
    {
        try
        {
            var banners = await _db.AppBanners
                .OrderBy(b => b.DisplayOrder)
                .ThenByDescending(b => b.CreatedAt)
                .ToListAsync();

            var bannerDtos = _mapper.Map<List<AppBannerDTO>>(banners);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Banners retrieved successfully",
                bannerDtos
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

    public async Task<ResponseData> GetActiveAsync()
    {
        try
        {
            var banners = await _db.AppBanners
                .Where(b => b.IsActive)
                .OrderBy(b => b.DisplayOrder)
                .ThenByDescending(b => b.CreatedAt)
                .ToListAsync();

            var bannerDtos = _mapper.Map<List<AppBannerDTO>>(banners);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Active banners retrieved successfully",
                bannerDtos
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
            var banner = await _db.AppBanners.FindAsync(id);

            if (banner == null)
            {
                return new ResponseData(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Banner not found",
                    null
                );
            }

            var bannerDto = _mapper.Map<AppBannerDTO>(banner);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Banner retrieved successfully",
                bannerDto
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

    public async Task<ResponseMessage> CreateAsync(AppBannerCreateDTO dto)
    {
        try
        {
            var banner = _mapper.Map<AppBanner>(dto);

            // Handle image upload
            if (dto.Image != null)
            {
                var uploadResult = await _imageService.UploadImageAsync(dto.Image, "banners");
                if (uploadResult.TaskStatus && uploadResult.Data != null)
                {
                    var imageData = uploadResult.Data as dynamic;
                    banner.ImageUrl = imageData?.ImageUrl;
                }
                else
                {
                    return new ResponseMessage(
                        System.Net.HttpStatusCode.BadRequest,
                        false,
                        "Failed to upload image"
                    );
                }
            }

            banner.CreatedAt = DateTime.Now;
            banner.UpdatedAt = DateTime.Now;

            await _db.AppBanners.AddAsync(banner);
            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Banner created successfully"
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

    public async Task<ResponseMessage> UpdateAsync(int id, AppBannerUpdateDTO dto)
    {
        try
        {
            var banner = await _db.AppBanners.FindAsync(id);

            if (banner == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Banner not found"
                );
            }

            // Handle image upload
            if (dto.Image != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(banner.ImageUrl))
                {
                    await _imageService.DeleteImageAsync(banner.ImageUrl);
                }

                // Upload new image
                var uploadResult = await _imageService.UploadImageAsync(dto.Image, "banners");
                if (uploadResult.TaskStatus && uploadResult.Data != null)
                {
                    var imageData = uploadResult.Data as dynamic;
                    banner.ImageUrl = imageData?.ImageUrl;
                }
            }

            // Update fields
            if (dto.Title != null) banner.Title = dto.Title;
            if (dto.LinkUrl != null) banner.LinkUrl = dto.LinkUrl;
            if (dto.DisplayOrder.HasValue) banner.DisplayOrder = dto.DisplayOrder.Value;
            if (dto.IsActive.HasValue) banner.IsActive = dto.IsActive.Value;

            banner.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Banner updated successfully"
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
            var banner = await _db.AppBanners.FindAsync(id);

            if (banner == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Banner not found"
                );
            }

            // Delete image if exists
            if (!string.IsNullOrEmpty(banner.ImageUrl))
            {
                await _imageService.DeleteImageAsync(banner.ImageUrl);
            }

            _db.AppBanners.Remove(banner);
            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Banner deleted successfully"
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

    public async Task<ResponseMessage> ToggleActiveStatusAsync(int id)
    {
        try
        {
            var banner = await _db.AppBanners.FindAsync(id);

            if (banner == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Banner not found"
                );
            }

            banner.IsActive = !banner.IsActive;
            banner.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                $"Banner {(banner.IsActive ? "activated" : "deactivated")} successfully"
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

    public async Task<ResponseMessage> UpdateDisplayOrderAsync(int id, int displayOrder)
    {
        try
        {
            var banner = await _db.AppBanners.FindAsync(id);

            if (banner == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "Banner not found"
                );
            }

            banner.DisplayOrder = displayOrder;
            banner.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Display order updated successfully"
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
