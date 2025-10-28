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

        public IFormFile? Image { get; set; }

        public bool? IsActive { get; set; }
    }
}
