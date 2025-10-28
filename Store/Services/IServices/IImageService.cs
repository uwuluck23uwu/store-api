using Microsoft.AspNetCore.Http;

namespace Store.Services.IServices
{
    public interface IImageService
    {
        Task<ResponseData> UploadImageAsync(IFormFile file, string folder);
        Task<ResponseMessage> DeleteImageAsync(string imageUrl);
        string GetImagePath(string fileName, string folder);
    }
}
