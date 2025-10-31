#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibrary.Models.Dto
{
    public class ProductDetailDTO
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

        // Category Info
        public string CategoryName { get; set; }

        // Seller Info
        public string ShopName { get; set; }

        public string ShopImageUrl { get; set; }

        public decimal? SellerRating { get; set; }

        public bool? IsVerifiedSeller { get; set; }

        public SellerDTO Seller { get; set; }

        // Reviews
        public List<ReviewDTO> Reviews { get; set; }

        // Product images
        public List<ProductImageDTO> ProductImages { get; set; }

        // Helper property to get all image URLs
        public List<string> ImageUrls => ProductImages?
            .OrderBy(img => img.DisplayOrder)
            .Select(img => img.ImageUrl)
            .ToList() ?? new List<string>();

        // Helper property to get primary image
        public string PrimaryImageUrl => ProductImages?.FirstOrDefault(img => img.IsPrimary)?.ImageUrl
                                         ?? ProductImages?.FirstOrDefault()?.ImageUrl
                                         ?? ImageUrl;
    }
}
