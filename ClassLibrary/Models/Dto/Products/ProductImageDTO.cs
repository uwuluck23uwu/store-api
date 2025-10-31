#nullable disable

namespace ClassLibrary.Models.Dto
{
    public class ProductImageDTO
    {
        public int ProductImageId { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; }
    }
}
