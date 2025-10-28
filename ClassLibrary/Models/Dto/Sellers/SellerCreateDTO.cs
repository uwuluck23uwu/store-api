#nullable disable
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ClassLibrary.Models.Dto
{
    public class SellerCreateDTO
    {
        [Required(ErrorMessage = "Shop name is required")]
        [MaxLength(200)]
        public string ShopName { get; set; }

        [MaxLength(1000)]
        public string ShopDescription { get; set; }

        public IFormFile ShopImage { get; set; }
    }
}
