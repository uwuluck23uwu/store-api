#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Data;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public string PhoneNumber { get; set; }

    public string Password { get; set; }

    public string PasswordHash { get; set; }

    public string ImageUrl { get; set; }

    public string Role { get; set; } // Admin, Seller, Customer

    public bool IsActive { get; set; } = true;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual Seller Seller { get; set; }
}
