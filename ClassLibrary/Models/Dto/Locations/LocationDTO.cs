#nullable disable
using System;

namespace ClassLibrary.Models.Dto
{
    public class LocationDTO
    {
        public int Id { get; set; }
        public string LocationId { get; set; } = string.Empty;
        public string RefId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string Description { get; set; }
        public string LocationType { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string ImageUrl { get; set; }
        public string IconUrl { get; set; }
        public string IconColor { get; set; }
        public int? SellerId { get; set; }
        public string SellerName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
