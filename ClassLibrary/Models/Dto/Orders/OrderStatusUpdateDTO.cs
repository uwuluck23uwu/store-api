#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class OrderStatusUpdateDTO
    {
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Pending|Confirmed|Preparing|Shipping|Delivered|Cancelled)$",
            ErrorMessage = "Invalid status. Must be: Pending, Confirmed, Preparing, Shipping, Delivered, or Cancelled")]
        public string Status { get; set; }
    }
}
