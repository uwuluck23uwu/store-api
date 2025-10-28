#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Data;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
