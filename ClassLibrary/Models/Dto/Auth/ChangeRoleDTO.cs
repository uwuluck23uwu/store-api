#nullable disable
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ClassLibrary.Models.Dto
{
    public class ChangeRoleDTO
    {
        [Required(ErrorMessage = "Role is required")]
        [JsonPropertyName("role")]
        public string Role { get; set; } // "Customer", "Seller", "Admin"
    }
}
