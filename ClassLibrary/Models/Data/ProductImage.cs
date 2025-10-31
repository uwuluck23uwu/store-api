#nullable disable
using System;

namespace ClassLibrary.Models.Data;

public partial class ProductImage
{
    public int ProductImageId { get; set; }

    public int ProductId { get; set; }

    public string ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Product Product { get; set; }
}
