using Microsoft.AspNetCore.Http;

namespace Store.Services.IServices;

public interface ILocationService
{
    // CRUD Operations
    Task<ResponseData> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null);
    Task<ResponseData> GetByIdAsync(int id);
    Task<ResponseData> GetByLocationIdAsync(string locationId);
    Task<ResponseData> CreateAsync(ClassLibrary.Models.Dto.LocationCreateDTO dto);
    Task<ResponseMessage> UpdateAsync(string refId, ClassLibrary.Models.Dto.LocationUpdateDTO dto);
    Task<ResponseMessage> DeleteAsync(int id);

    // Filter Operations
    Task<ResponseData> GetByTypeAsync(string locationType);
    Task<ResponseData> GetBySellerIdAsync(int sellerId);

    // Location-based Search
    Task<ResponseData> GetNearbyLocationsAsync(decimal latitude, decimal longitude, decimal radiusKm);

    // Image Upload
    Task<ResponseData> UploadLocationImageAsync(int locationId, IFormFile file);

    // Utility
    Task<string> GenerateLocationIdAsync();
}
