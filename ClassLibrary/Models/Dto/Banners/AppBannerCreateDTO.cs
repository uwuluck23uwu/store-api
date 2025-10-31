#nullable disable
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ClassLibrary.Models.Dto
{
    public class AppBannerCreateDTO
    {
        [MaxLength(200)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public IFormFile Image { get; set; }

        [MaxLength(500)]
        public string LinkUrl { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}
