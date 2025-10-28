#nullable disable
using System;

namespace ClassLibrary.Models.Dto
{
    public class ImageDTO
    {
        public int ImageId { get; set; }

        public string ImageUrl { get; set; }

        public string ImageType { get; set; }

        public int? ReferenceId { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
