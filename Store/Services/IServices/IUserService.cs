using ClassLibrary.Models.Dto;
using ClassLibrary.Models.Response;
using Microsoft.AspNetCore.Http;

namespace Store.Services.IServices;

public interface IUserService
{
    Task<ResponseData> GetUserProfileAsync(int userId);
    Task<ResponseMessage> UpdateProfileAsync(int userId, UpdateProfileDTO dto);
    Task<ResponseData> UploadAvatarAsync(int userId, IFormFile file);
    Task<ResponseMessage> ChangeRoleAsync(int userId, ChangeRoleDTO dto);
}
