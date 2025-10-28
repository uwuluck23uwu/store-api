using ClassLibrary.Models.Dto;

namespace Store.Services.IServices
{
    public interface IAddressService
    {
        Task<ResponseData> GetByUserIdAsync(int userId);
        Task<ResponseData> GetByIdAsync(int addressId);
        Task<ResponseMessage> CreateAsync(int userId, AddressCreateDTO dto);
        Task<ResponseMessage> UpdateAsync(int addressId, int userId, AddressUpdateDTO dto);
        Task<ResponseMessage> DeleteAsync(int addressId, int userId);
        Task<ResponseMessage> SetDefaultAsync(int userId, int addressId);
        Task<ResponseData> GetDefaultAddressAsync(int userId);
    }
}
