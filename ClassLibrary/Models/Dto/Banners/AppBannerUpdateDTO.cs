#nullable disable
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ClassLibrary.Models.Dto
{
    public class AppBannerUpdateDTO
    {
        [MaxLength(200)]
        public string Title { get; set; }

        public IFormFile Image { get; set; }

        [MaxLength(500)]
        public string LinkUrl { get; set; }

        public int? DisplayOrder { get; set; }

        public bool? IsActive { get; set; }
    }
}
