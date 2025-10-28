#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class PaymentCreateDTO
    {
        [Required(ErrorMessage = "Order ID is required")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [MaxLength(50)]
        public string Method { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [MaxLength(100)]
        public string ReferenceCode { get; set; }
    }
}
