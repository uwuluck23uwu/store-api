#nullable disable
using System;

namespace ClassLibrary.Models.Data;

public partial class AppBanner
{
    public int AppBannerId { get; set; }

    public string Title { get; set; }

    public string ImageUrl { get; set; }

    public string LinkUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
