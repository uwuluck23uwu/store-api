#nullable disable
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ClassLibrary.Models.Dto
{
    public class SellerUpdateDTO
    {
        [MaxLength(200)]
        public string ShopName { get; set; }

        [MaxLength(1000)]
        public string ShopDescription { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(255)]
        public string ShopImageUrl { get; set; }

        [MaxLength(255)]
        public string LogoUrl { get; set; }

        [MaxLength(255)]
        public string QrCodeUrl { get; set; }
    }
}
