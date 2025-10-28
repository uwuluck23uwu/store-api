#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Data;

public partial class Otp
{
    public int OtpId { get; set; }

    public string Email { get; set; }

    public string Code { get; set; }

    public string Type { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public bool? IsUsed { get; set; }

    public bool? IsActive { get; set; }

    public int? AttemptsCount { get; set; }

    public int? MaxAttempts { get; set; }

    public string TempUserData { get; set; }

    public string IpAddress { get; set; }

    public string UserAgent { get; set; }

    public int? ResendCount { get; set; }

    public int? MaxResendCount { get; set; }

    public DateTime? LastResendAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
