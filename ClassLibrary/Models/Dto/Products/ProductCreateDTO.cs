#nullable disable
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ClassLibrary.Models.Dto
{
    public class ProductCreateDTO
    {
        [Required(ErrorMessage = "Category ID is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(200)]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(2000)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        [MaxLength(50)]
        public string Unit { get; set; } = "ชิ้น";

        public IFormFile Image { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
