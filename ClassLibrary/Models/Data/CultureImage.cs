#nullable disable
using System;

namespace ClassLibrary.Models.Data;

public partial class CultureImage
{
    public int CultureImageId { get; set; }

    public int CultureId { get; set; }

    public string ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Culture Culture { get; set; }
}
