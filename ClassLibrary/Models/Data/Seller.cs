#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Data;

public partial class Seller
{
    public int SellerId { get; set; }

    public int UserId { get; set; }

    public string ShopName { get; set; }

    public string ShopDescription { get; set; }

    public string ShopImageUrl { get; set; }

    public string LogoUrl { get; set; }

    public string QrCodeUrl { get; set; }

    public string Description { get; set; }

    public string PhoneNumber { get; set; }

    public string Address { get; set; }

    public decimal? Rating { get; set; }

    public int? TotalSales { get; set; }

    public bool? IsVerified { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();
}
