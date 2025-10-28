#nullable disable
using System;
using System.Text.Json.Serialization;

namespace ClassLibrary.Models.Dto
{
    public class UserDTO
    {
        [JsonPropertyName("id")]
        public int UserId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string Phone { get; set; }

        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // If user is a seller
        [JsonPropertyName("seller")]
        public SellerDTO Seller { get; set; }
    }
}
