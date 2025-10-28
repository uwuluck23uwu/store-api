#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class UpdateCartItemDTO
    {
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
