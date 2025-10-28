#nullable disable
using System;

namespace ClassLibrary.Models.Dto
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int ProductCount { get; set; }
    }
}
