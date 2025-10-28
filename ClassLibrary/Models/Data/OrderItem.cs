#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Data;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int SellerId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Price { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public virtual Order Order { get; set; }

    public virtual Product Product { get; set; }

    public virtual Seller Seller { get; set; }
}
