#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Data;

public partial class Cart
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.Now;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public virtual User User { get; set; }

    public virtual Product Product { get; set; }
}
