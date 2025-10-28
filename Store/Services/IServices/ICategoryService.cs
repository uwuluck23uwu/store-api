using ClassLibrary.Models.Dto;

namespace Store.Services.IServices
{
    public interface ICategoryService
    {
        Task<ResponseData> GetAllAsync();
        Task<ResponseData> GetByIdAsync(int id);
        Task<ResponseMessage> CreateAsync(CategoryCreateDTO dto);
        Task<ResponseMessage> UpdateAsync(int id, CategoryUpdateDTO dto);
        Task<ResponseMessage> DeleteAsync(int id);
        Task<ResponseData> GetActiveCategoriesAsync();
    }
}
