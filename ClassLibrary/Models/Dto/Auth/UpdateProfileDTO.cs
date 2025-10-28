#nullable disable
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ClassLibrary.Models.Dto
{
    public class UpdateProfileDTO
    {
        [Required(ErrorMessage = "FirstName is required")]
        [MaxLength(50)]
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        [MaxLength(50)]
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [MaxLength(20)]
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }
    }
}
