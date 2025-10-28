using ClassLibrary.Models.Dto;

namespace Store.Services.IServices
{
    public interface IReviewService
    {
        Task<ResponseData> GetByProductIdAsync(int productId);
        Task<ResponseMessage> CreateAsync(int userId, ReviewCreateDTO dto);
        Task<ResponseMessage> UpdateAsync(int reviewId, int userId, ReviewUpdateDTO dto);
        Task<ResponseMessage> DeleteAsync(int reviewId, int userId);
        Task<bool> CanReviewAsync(int userId, int productId, int orderId);
    }
}
