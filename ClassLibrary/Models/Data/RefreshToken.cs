#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Data;

public partial class RefreshToken
{
    public int RefreshTokenId { get; set; }

    public int UserId { get; set; }

    public string JwtTokenId { get; set; }

    public string Token { get; set; }

    public bool? IsValid { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public virtual User User { get; set; }
}
