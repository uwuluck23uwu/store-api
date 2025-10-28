#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Dto
{
    public class SellerDetailDTO
    {
        public int SellerId { get; set; }

        public int UserId { get; set; }

        public string ShopName { get; set; }

        public string ShopDescription { get; set; }

        public string ShopImageUrl { get; set; }

        public string LogoUrl { get; set; }

        public string Description { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public decimal? Rating { get; set; }

        public int? TotalSales { get; set; }

        public bool? IsVerified { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string OwnerName { get; set; }

        public string UserEmail { get; set; }

        public string QrCodeUrl { get; set; }

        // Products for this seller
        public List<ProductDTO> Products { get; set; }
    }
}
