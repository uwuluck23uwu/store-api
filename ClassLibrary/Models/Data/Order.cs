#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Data;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public string OrderNumber { get; set; } = string.Empty; // ORD-YYYYMMDD-XXXX

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Preparing, Shipping, Delivered, Cancelled

    public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Refunded

    public string Note { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public virtual User User { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Payment Payment { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<SellerRevenue> SellerRevenues { get; set; } = new List<SellerRevenue>();
}
