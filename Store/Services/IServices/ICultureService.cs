using ClassLibrary.Models.Dto;
using ClassLibrary.Models.Dto.Cultures;

namespace Store.Services.IServices
{
    public interface ICultureService
    {
        Task<ResponseData> GetAllAsync();
        Task<ResponseData> GetByIdAsync(int id);
        Task<ResponseMessage> CreateAsync(CultureCreateDTO dto);
        Task<ResponseMessage> UpdateAsync(int id, CultureUpdateDTO dto);
        Task<ResponseMessage> DeleteAsync(int id);
        Task<ResponseData> GetActiveCulturesAsync();
        Task<ResponseMessage> ToggleActiveStatusAsync(int id);
    }
}
