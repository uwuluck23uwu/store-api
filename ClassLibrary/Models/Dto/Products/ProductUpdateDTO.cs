using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ClassLibrary.Models.Dto
{
    public class ProductUpdateDTO
    {
        public int? CategoryId { get; set; }

        [MaxLength(200)]
        public string? ProductName { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int? Stock { get; set; }

        [MaxLength(50)]
        public string? Unit { get; set; }

        // For backward compatibility - single image
        public IFormFile? Image { get; set; }

        // Multiple images support
        public List<IFormFile>? Images { get; set; }

        // Image IDs to delete
        public List<int>? DeleteImageIds { get; set; }

        public bool? IsActive { get; set; }
    }
}
