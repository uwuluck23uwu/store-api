using Microsoft.EntityFrameworkCore;
using ClassLibrary.Models.Data;
using ClassLibrary.Models.Dto;
using ClassLibrary.Models.Response;
using Store.Data;
using Store.Services.IServices;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Store.Services;

public class UserService : Service<User>, IUserService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UserService> _logger;

    public UserService(
        ApplicationDbContext db,
        AutoMapper.IMapper mapper,
        IWebHostEnvironment env,
        ILogger<UserService> logger)
        : base(db, mapper)
    {
        _env = env;
        _logger = logger;
    }

    public async Task<ResponseData> GetUserProfileAsync(int userId)
    {
        try
        {
            var user = await _db.Users
                .Where(u => u.UserId == userId && u.IsActive)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return new ResponseData(
                    HttpStatusCode.NotFound,
                    false,
                    "User not found",
                    null
                );
            }

            var userDTO = _mapper.Map<UserDTO>(user);

            return new ResponseData(
                HttpStatusCode.OK,
                true,
                "User profile retrieved successfully",
                userDTO
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            return new ResponseData(
                HttpStatusCode.InternalServerError,
                false,
                "An error occurred while getting user profile",
                null
            );
        }
    }

    public async Task<ResponseMessage> UpdateProfileAsync(int userId, UpdateProfileDTO dto)
    {
        try
        {
            var user = await _db.Users
                .Where(u => u.UserId == userId && u.IsActive)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return new ResponseMessage(
                    HttpStatusCode.NotFound,
                    false,
                    "User not found"
                );
            }

            // Update user profile
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Name = $"{dto.FirstName} {dto.LastName}"; // Update Name based on FirstName + LastName
            user.PhoneNumber = dto.PhoneNumber;
            user.Phone = dto.PhoneNumber; // Update Phone to match PhoneNumber
            user.UpdatedAt = DateTime.UtcNow;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return new ResponseMessage(
                HttpStatusCode.OK,
                true,
                "Profile updated successfully"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile");
            return new ResponseMessage(
                HttpStatusCode.InternalServerError,
                false,
                "An error occurred while updating profile"
            );
        }
    }

    public async Task<ResponseData> UploadAvatarAsync(int userId, IFormFile file)
    {
        try
        {
            var user = await _db.Users
                .Where(u => u.UserId == userId && u.IsActive)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return new ResponseData(
                    HttpStatusCode.NotFound,
                    false,
                    "User not found",
                    null
                );
            }

            // Create avatars directory if it doesn't exist
            var avatarsPath = Path.Combine(_env.WebRootPath, "images", "avatars");
            if (!Directory.Exists(avatarsPath))
            {
                Directory.CreateDirectory(avatarsPath);
            }

            // Delete old avatar if exists
            if (!string.IsNullOrEmpty(user.ImageUrl))
            {
                var oldFilePath = Path.Combine(_env.WebRootPath, user.ImageUrl.TrimStart('/'));
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            // Generate unique filename
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"user-{userId}-{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(avatarsPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Update user's ImageUrl
            var imageUrl = $"/images/avatars/{fileName}";
            user.ImageUrl = imageUrl;
            user.UpdatedAt = DateTime.UtcNow;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return new ResponseData(
                HttpStatusCode.OK,
                true,
                "Avatar uploaded successfully",
                imageUrl
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading avatar");
            return new ResponseData(
                HttpStatusCode.InternalServerError,
                false,
                "An error occurred while uploading avatar",
                null
            );
        }
    }

    public async Task<ResponseMessage> ChangeRoleAsync(int userId, ChangeRoleDTO dto)
    {
        try
        {
            var user = await _db.Users
                .Where(u => u.UserId == userId && u.IsActive)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return new ResponseMessage(
                    HttpStatusCode.NotFound,
                    false,
                    "User not found"
                );
            }

            // Validate role
            var validRoles = new[] { "Customer", "Seller", "Admin" };
            if (!validRoles.Contains(dto.Role))
            {
                return new ResponseMessage(
                    HttpStatusCode.BadRequest,
                    false,
                    "Invalid role. Valid roles are: Customer, Seller, Admin"
                );
            }

            // Update user role
            user.Role = dto.Role;
            user.UpdatedAt = DateTime.UtcNow;

            _db.Users.Update(user);

            // If changing to Seller or Admin role, create a Seller record if it doesn't exist
            if (dto.Role == "Seller" || dto.Role == "Admin")
            {
                var existingSeller = await _db.Sellers
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (existingSeller == null)
                {
                    var newSeller = new Seller
                    {
                        UserId = userId,
                        ShopName = $"{user.FirstName} {user.LastName}'s Shop",
                        ShopDescription = "Welcome to my shop!",
                        IsActive = true,
                        IsVerified = dto.Role == "Admin", // Auto-verify Admin sellers
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _db.Sellers.Add(newSeller);
                }
                else if (dto.Role == "Admin" && existingSeller.IsVerified != true)
                {
                    // Auto-verify existing seller when changing to Admin
                    existingSeller.IsVerified = true;
                    existingSeller.UpdatedAt = DateTime.UtcNow;
                    _db.Sellers.Update(existingSeller);
                }
            }

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                HttpStatusCode.OK,
                true,
                $"Role changed to {dto.Role} successfully"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing role");
            return new ResponseMessage(
                HttpStatusCode.InternalServerError,
                false,
                "An error occurred while changing role"
            );
        }
    }
}
