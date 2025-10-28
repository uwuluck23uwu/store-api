#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class LocationUpdateDTO
    {
        public string RefId { get; set; }

        [StringLength(200, ErrorMessage = "Location name cannot exceed 200 characters")]
        public string LocationName { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        [StringLength(50, ErrorMessage = "Location type cannot exceed 50 characters")]
        public string LocationType { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal? Longitude { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string Address { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string PhoneNumber { get; set; }

        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string ImageUrl { get; set; }

        [StringLength(500, ErrorMessage = "Icon URL cannot exceed 500 characters")]
        public string IconUrl { get; set; }

        [StringLength(50, ErrorMessage = "Icon color cannot exceed 50 characters")]
        public string IconColor { get; set; }

        public bool? IsActive { get; set; }
    }
}
