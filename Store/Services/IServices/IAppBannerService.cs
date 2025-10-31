using ClassLibrary.Models.Dto;

namespace Store.Services.IServices
{
    public interface IAppBannerService
    {
        Task<ResponseData> GetAllAsync();
        Task<ResponseData> GetActiveAsync();
        Task<ResponseData> GetByIdAsync(int id);
        Task<ResponseMessage> CreateAsync(AppBannerCreateDTO dto);
        Task<ResponseMessage> UpdateAsync(int id, AppBannerUpdateDTO dto);
        Task<ResponseMessage> DeleteAsync(int id);
        Task<ResponseMessage> ToggleActiveStatusAsync(int id);
        Task<ResponseMessage> UpdateDisplayOrderAsync(int id, int displayOrder);
    }
}
