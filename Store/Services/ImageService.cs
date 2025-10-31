using System.Net;
using Microsoft.AspNetCore.Http;

namespace Store.Services
{
    public class ImageService : IServices.IImageService
    {
        private readonly IWebHostEnvironment _env;

        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<ResponseData> UploadImageAsync(IFormFile file, string folder)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new ResponseData(
                        HttpStatusCode.BadRequest,
                        false,
                        "No file uploaded",
                        null
                    );
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    return new ResponseData(
                        HttpStatusCode.BadRequest,
                        false,
                        "Invalid file type. Only JPG, JPEG, PNG, GIF, WEBP are allowed",
                        null
                    );
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{extension}";
                var folderPath = Path.Combine(_env.WebRootPath, "images", folder);

                // Create folder if not exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imageUrl = $"/images/{folder}/{fileName}";

                return new ResponseData(
                    HttpStatusCode.OK,
                    true,
                    "Image uploaded successfully",
                    new { ImageUrl = imageUrl }
                );
            }
            catch (Exception ex)
            {
                return new ResponseData(
                    HttpStatusCode.InternalServerError,
                    false,
                    $"Error uploading image: {ex.Message}",
                    null
                );
            }
        }

        public async Task<ResponseMessage> DeleteImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return new ResponseMessage(
                        HttpStatusCode.BadRequest,
                        false,
                        "Image URL is required"
                    );
                }

                var filePath = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/'));

                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                }

                return new ResponseMessage(
                    HttpStatusCode.OK,
                    true,
                    "Image deleted successfully"
                );
            }
            catch (Exception ex)
            {
                return new ResponseMessage(
                    HttpStatusCode.InternalServerError,
                    false,
                    $"Error deleting image: {ex.Message}"
                );
            }
        }

        public string GetImagePath(string fileName, string folder)
        {
            return $"/images/{folder}/{fileName}";
        }
    }
}
