#nullable disable
using System;

namespace ClassLibrary.Models.Dto
{
    public class CartItemDTO
    {
        public int CartId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductImageUrl { get; set; }

        public string ImageUrl { get; set; }

        public decimal? Price { get; set; }

        public int? Quantity { get; set; }

        public string Unit { get; set; }

        public int? Stock { get; set; }

        public bool? IsActive { get; set; }

        public decimal? Subtotal { get; set; }

        public DateTime? AddedAt { get; set; }

        // Seller Info
        public int SellerId { get; set; }

        public string ShopName { get; set; }

        public string SellerName { get; set; }

        // Category Info
        public string CategoryName { get; set; }
    }
}
