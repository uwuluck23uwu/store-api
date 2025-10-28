#nullable disable
using System;

namespace ClassLibrary.Models.Dto
{
    public class ProductListDTO
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal? Price { get; set; }

        public int? Stock { get; set; }

        public string Unit { get; set; }

        public string ImageUrl { get; set; }

        public decimal? Rating { get; set; }

        public int? TotalReviews { get; set; }

        public bool? IsActive { get; set; }

        public string CategoryName { get; set; }

        public string ShopName { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
