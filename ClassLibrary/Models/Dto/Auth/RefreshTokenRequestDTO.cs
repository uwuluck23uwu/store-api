#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class RefreshTokenRequestDTO
    {
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; }
    }
}
