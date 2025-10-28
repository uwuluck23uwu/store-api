using ClassLibrary.Models.Dto;

namespace Store.Services.IServices
{
    public interface ICartService
    {
        Task<ResponseData> GetByUserIdAsync(int userId);
        Task<ResponseMessage> AddToCartAsync(int userId, AddToCartDTO dto);
        Task<ResponseMessage> UpdateQuantityAsync(int cartId, int userId, UpdateCartItemDTO dto);
        Task<ResponseMessage> RemoveFromCartAsync(int cartId, int userId);
        Task<ResponseMessage> ClearCartAsync(int userId);
        Task<int> GetCartCountAsync(int userId);
    }
}
