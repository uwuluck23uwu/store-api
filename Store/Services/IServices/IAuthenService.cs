using ClassLibrary.Models.Dto;

namespace Store.Services.IServices
{
    public interface IAuthenService
    {
        Task<ResponseData> RegisterAsync(RegisterRequestDTO dto);
        Task<ResponseData> LoginAsync(LoginRequestDTO dto);
        Task<ResponseData> RefreshTokenAsync(string refreshToken);
        Task<ResponseMessage> LogoutAsync(int userId);
        Task<ResponseData> GetProfileAsync(int userId);
        Task<ResponseMessage> UpdateProfileAsync(int userId, UpdateProfileDTO dto);
        Task<ResponseMessage> ChangePasswordAsync(int userId, ChangePasswordDTO dto);
    }
}
