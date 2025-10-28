#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class RegisterRequestDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [MaxLength(20)]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        [MaxLength(20)]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [MaxLength(255)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        public string Role { get; set; } = "Customer"; // Customer, Seller, Admin
    }
}
