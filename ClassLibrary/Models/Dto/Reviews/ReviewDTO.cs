#nullable disable
using System;

namespace ClassLibrary.Models.Dto
{
    public class ReviewDTO
    {
        public int ReviewId { get; set; }

        public int ProductId { get; set; }

        public int UserId { get; set; }

        public int? OrderId { get; set; }

        public int? Rating { get; set; } // 1-5

        public string Comment { get; set; }

        public string ImageUrl { get; set; }

        public DateTime? CreatedAt { get; set; }

        // User Info
        public string UserName { get; set; }

        public string UserImageUrl { get; set; }

        // Product Info
        public string ProductName { get; set; }
    }
}
