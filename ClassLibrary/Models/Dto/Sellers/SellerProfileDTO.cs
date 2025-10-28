#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Dto
{
    public class SellerProfileDTO
    {
        public int SellerId { get; set; }

        public string ShopName { get; set; }

        public string ShopDescription { get; set; }

        public string ShopImageUrl { get; set; }

        public decimal? Rating { get; set; }

        public int? TotalSales { get; set; }

        public bool? IsVerified { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int TotalProducts { get; set; }

        public int TotalOrders { get; set; }

        public List<ProductDTO> Products { get; set; }
    }
}
