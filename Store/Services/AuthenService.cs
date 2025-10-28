using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ClassLibrary.Models.Data;
using ClassLibrary.Models.Dto;
using Store.Data;

namespace Store.Services;

public class AuthenService : Service<User>, IServices.IAuthenService
{
    private readonly IConfiguration _configuration;

    public AuthenService(ApplicationDbContext db, AutoMapper.IMapper mapper, IConfiguration configuration)
        : base(db, mapper)
    {
        _configuration = configuration;
    }

    public async Task<ResponseData> RegisterAsync(RegisterRequestDTO dto)
    {
        try
        {
            // Check if email already exists
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                return new ResponseData(
                    System.Net.HttpStatusCode.BadRequest,
                    false,
                    "Email already exists",
                    null
                );
            }

            // Hash password with BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Create new user
            var user = new User
            {
                Name = dto.Name,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = passwordHash,
                Role = dto.Role ?? "Customer", // Default to Customer
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            // Generate JWT token
            var tokenResponse = GenerateJwtToken(user);

            var userDto = _mapper.Map<UserDTO>(user);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "User registered successfully",
                new
                {
                    user = userDto,
                    token = tokenResponse.AccessToken,
                    refreshToken = tokenResponse.RefreshToken
                }
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}",
                null
            );
        }
    }

    public async Task<ResponseData> LoginAsync(LoginRequestDTO dto)
    {
        try
        {
            // Find user by email
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
            {
                return new ResponseData(
                    System.Net.HttpStatusCode.Unauthorized,
                    false,
                    "Invalid email or password",
                    null
                );
            }

            // Verify password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return new ResponseData(
                    System.Net.HttpStatusCode.Unauthorized,
                    false,
                    "Invalid email or password",
                    null
                );
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return new ResponseData(
                    System.Net.HttpStatusCode.Forbidden,
                    false,
                    "User account is inactive",
                    null
                );
            }

            // Generate JWT token
            var tokenResponse = GenerateJwtToken(user);

            // Save refresh token
            var refreshToken = new RefreshToken
            {
                UserId = user.UserId,
                Token = tokenResponse.RefreshToken,
                ExpiresAt = DateTime.Now.AddMinutes(_configuration.GetValue<int>("JwtSettings:RefreshTokenMinutes")),
                CreatedAt = DateTime.Now
            };

            await _db.RefreshTokens.AddAsync(refreshToken);
            await _db.SaveChangesAsync();

            var userDto = _mapper.Map<UserDTO>(user);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Login successful",
                new
                {
                    user = userDto,
                    token = tokenResponse.AccessToken,
                    refreshToken = tokenResponse.RefreshToken
                }
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}",
                null
            );
        }
    }

    public async Task<ResponseData> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            // Find refresh token
            var refreshTokenEntity = await _db.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);

            if (refreshTokenEntity == null || refreshTokenEntity.ExpiresAt < DateTime.Now)
            {
                return new ResponseData(
                    System.Net.HttpStatusCode.Unauthorized,
                    false,
                    "Invalid or expired refresh token",
                    null
                );
            }

            // Generate new tokens
            var tokenResponse = GenerateJwtToken(refreshTokenEntity.User);

            // Revoke old refresh token
            refreshTokenEntity.IsRevoked = true;

            // Create new refresh token
            var newRefreshToken = new RefreshToken
            {
                UserId = refreshTokenEntity.UserId,
                Token = tokenResponse.RefreshToken,
                ExpiresAt = DateTime.Now.AddMinutes(_configuration.GetValue<int>("JwtSettings:RefreshTokenMinutes")),
                CreatedAt = DateTime.Now
            };

            await _db.RefreshTokens.AddAsync(newRefreshToken);
            await _db.SaveChangesAsync();

            var userDto = _mapper.Map<UserDTO>(refreshTokenEntity.User);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Token refreshed successfully",
                new { User = userDto, Token = tokenResponse }
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}",
                null
            );
        }
    }

    public async Task<ResponseMessage> LogoutAsync(int userId)
    {
        try
        {
            // Revoke all refresh tokens for user
            var refreshTokens = await _db.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in refreshTokens)
            {
                token.IsRevoked = true;
            }

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Logout successful"
            );
        }
        catch (Exception ex)
        {
            return new ResponseMessage(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}"
            );
        }
    }

    public async Task<ResponseData> GetProfileAsync(int userId)
    {
        try
        {
            var user = await _db.Users
                .Include(u => u.Seller)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return new ResponseData(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "User not found",
                    null
                );
            }

            var userDto = _mapper.Map<UserDTO>(user);

            return new ResponseData(
                System.Net.HttpStatusCode.OK,
                true,
                "Profile retrieved successfully",
                userDto
            );
        }
        catch (Exception ex)
        {
            return new ResponseData(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}",
                null
            );
        }
    }

    public async Task<ResponseMessage> UpdateProfileAsync(int userId, UpdateProfileDTO dto)
    {
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "User not found"
                );
            }

            // Update user fields
            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
            user.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Profile updated successfully"
            );
        }
        catch (Exception ex)
        {
            return new ResponseMessage(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}"
            );
        }
    }

    public async Task<ResponseMessage> ChangePasswordAsync(int userId, ChangePasswordDTO dto)
    {
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.NotFound,
                    false,
                    "User not found"
                );
            }

            // Verify old password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash);
            if (!isPasswordValid)
            {
                return new ResponseMessage(
                    System.Net.HttpStatusCode.BadRequest,
                    false,
                    "Current password is incorrect"
                );
            }

            // Hash new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return new ResponseMessage(
                System.Net.HttpStatusCode.OK,
                true,
                "Password changed successfully"
            );
        }
        catch (Exception ex)
        {
            return new ResponseMessage(
                System.Net.HttpStatusCode.InternalServerError,
                false,
                $"Error: {ex.Message}"
            );
        }
    }

    private TokenDTO GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = _configuration.GetValue<string>("ApiKey");
        var keyBytes = Encoding.ASCII.GetBytes(key ?? "");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.GetValue<int>("AccessTokenMinutes")),
            Issuer = jwtSettings.GetValue<string>("Issuer"),
            Audience = jwtSettings.GetValue<string>("Audience"),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        // Generate refresh token
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        return new TokenDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = tokenDescriptor.Expires.Value
        };
    }
}
