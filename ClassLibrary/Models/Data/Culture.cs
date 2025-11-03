#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Data;

public partial class Culture
{
    public int CultureId { get; set; }

    public string CultureName { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public string Category { get; set; } // ประเภทวัฒนธรรม เช่น ศิลปกรรม, ประเพณี, วรรณกรรม

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CultureImage> CultureImages { get; set; } = new List<CultureImage>();
}
