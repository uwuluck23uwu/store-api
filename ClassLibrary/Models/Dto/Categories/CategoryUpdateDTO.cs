#nullable disable
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ClassLibrary.Models.Dto
{
    public class CategoryUpdateDTO
    {
        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100)]
        public string CategoryName { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public IFormFile Image { get; set; }

        public bool IsActive { get; set; }
    }
}
