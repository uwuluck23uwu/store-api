#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Data;

public partial class Image
{
    public int ImageId { get; set; }

    public string RefId { get; set; }

    public string ImageUrl { get; set; }

    public DateTime? UploadedDate { get; set; }
}
