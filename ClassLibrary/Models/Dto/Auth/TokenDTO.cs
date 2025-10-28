#nullable disable
using System;
using System.Text.Json.Serialization;

namespace ClassLibrary.Models.Dto
{
    public class TokenDTO
    {
        [JsonPropertyName("token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("expiresAt")]
        public DateTime ExpiresAt { get; set; }

        [JsonPropertyName("tokenType")]
        public string TokenType { get; set; } = "Bearer";
    }
}
