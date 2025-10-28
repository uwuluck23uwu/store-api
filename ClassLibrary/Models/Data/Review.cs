#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Data;

public partial class Review
{
    public int ReviewId { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public int? OrderId { get; set; }

    public int? Rating { get; set; } // 1-5

    public string Comment { get; set; }

    public string ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Product Product { get; set; }

    public virtual User User { get; set; }

    public virtual Order Order { get; set; }
}
