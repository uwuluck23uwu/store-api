#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibrary.Models.Dto
{
    public class ProductDTO
    {
        public int ProductId { get; set; }

        public int SellerId { get; set; }

        public int CategoryId { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public decimal? Price { get; set; }

        public int? Stock { get; set; }

        public string Unit { get; set; }

        public string ImageUrl { get; set; }

        public decimal? Rating { get; set; }

        public int? TotalReviews { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Additional properties for display
        public string CategoryName { get; set; }

        public string SellerName { get; set; }

        // Product images
        public List<ProductImageDTO> ProductImages { get; set; }

        // Helper property to get first image or default ImageUrl
        public string PrimaryImageUrl => ProductImages?.FirstOrDefault(img => img.IsPrimary)?.ImageUrl
                                         ?? ProductImages?.FirstOrDefault()?.ImageUrl
                                         ?? ImageUrl;
    }
}
