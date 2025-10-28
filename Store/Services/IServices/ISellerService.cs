using ClassLibrary.Models.Dto;
using Microsoft.AspNetCore.Http;

namespace Store.Services.IServices
{
    public interface ISellerService
    {
        Task<ResponseData> GetAllAsync();
        Task<ResponseData> GetProfileAsync(int sellerId);
        Task<ResponseMessage> RegisterSellerAsync(int userId, SellerCreateDTO dto);
        Task<ResponseMessage> UpdateProfileAsync(int sellerId, SellerUpdateDTO dto);
        Task<ResponseData> GetByUserIdAsync(int userId);
        Task<ResponseMessage> UpdateRatingAsync(int sellerId);
        Task<ResponseData> UploadShopImageAsync(int userId, IFormFile file);
        Task<ResponseData> UploadLogoAsync(int userId, IFormFile file);
        Task<ResponseData> UploadQRCodeAsync(int userId, IFormFile file);
    }
}
