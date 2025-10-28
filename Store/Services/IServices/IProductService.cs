using ClassLibrary.Models.Dto;

namespace Store.Services.IServices
{
    public interface IProductService
    {
        Task<ResponseData> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, int? categoryId = null, int? sellerId = null);
        Task<ResponseData> GetByIdAsync(int productId);
        Task<ResponseMessage> CreateAsync(int sellerId, ProductCreateDTO dto);
        Task<ResponseMessage> UpdateAsync(int productId, int sellerId, ProductUpdateDTO dto);
        Task<ResponseMessage> DeleteAsync(int productId, int sellerId);
        Task<ResponseData> GetByCategoryAsync(int categoryId);
        Task<ResponseData> GetBySellerAsync(int sellerId);
        Task<ResponseData> SearchAsync(string keyword);
        Task<ResponseMessage> UpdateStockAsync(int productId, int quantity);
    }
}
