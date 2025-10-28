#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class AddressCreateDTO
    {
        [Required(ErrorMessage = "Receiver name is required")]
        [MaxLength(100)]
        public string ReceiverName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(20)]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address line 1 is required")]
        [MaxLength(500)]
        public string AddressLine1 { get; set; }

        [MaxLength(500)]
        public string AddressLine2 { get; set; }

        [Required(ErrorMessage = "District is required")]
        [MaxLength(100)]
        public string District { get; set; } // ตำบล

        [Required(ErrorMessage = "Subdistrict is required")]
        [MaxLength(100)]
        public string Subdistrict { get; set; } // อำเภอ

        [Required(ErrorMessage = "Province is required")]
        [MaxLength(100)]
        public string Province { get; set; }

        [Required(ErrorMessage = "Postal code is required")]
        [MaxLength(10)]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Postal code must be 5 digits")]
        public string PostalCode { get; set; }

        public bool IsDefault { get; set; } = false;
    }
}
