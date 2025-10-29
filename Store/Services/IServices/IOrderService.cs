using ClassLibrary.Models.Dto;

namespace Store.Services.IServices
{
    public interface IOrderService
    {
        Task<ResponseData> GetByUserIdAsync(int userId);
        Task<ResponseData> GetBySellerIdAsync(int sellerId);
        Task<ResponseData> GetByIdAsync(int orderId);
        Task<ResponseData> GetAllOrdersAsync();
        Task<ResponseData> CreateAsync(int userId, OrderCreateDTO dto);
        Task<ResponseMessage> UpdateStatusAsync(int orderId, string status, int userId);
        Task<ResponseMessage> CancelOrderAsync(int orderId, int userId);
    }
}
