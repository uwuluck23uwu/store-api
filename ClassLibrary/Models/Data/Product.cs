#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Data;

public partial class Product
{
    public int ProductId { get; set; }

    public int SellerId { get; set; }

    public int CategoryId { get; set; }

    public string ProductName { get; set; }

    public string Description { get; set; }

    public decimal? Price { get; set; }

    public int? Stock { get; set; }

    public string Unit { get; set; } // กิโลกรัม, ห่อ, ชิ้น

    public string ImageUrl { get; set; }

    public decimal? Rating { get; set; }

    public int? TotalReviews { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Seller Seller { get; set; }

    public virtual Category Category { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
}
