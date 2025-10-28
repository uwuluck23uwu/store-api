#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Data;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public string Method { get; set; }

    public decimal? Amount { get; set; }

    public string Status { get; set; }

    public string ReferenceCode { get; set; }

    public DateTime? PaidAt { get; set; }

    public virtual Order Order { get; set; }
}
