#nullable disable
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ClassLibrary.Models.Dto
{
    public class ReviewUpdateDTO
    {
        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [MaxLength(2000)]
        public string Comment { get; set; }

        public IFormFile Image { get; set; }
    }
}
