#nullable disable
using System;

namespace ClassLibrary.Models.Dto
{
    public class AddressDTO
    {
        public int AddressId { get; set; }

        public int UserId { get; set; }

        public string ReceiverName { get; set; }

        public string PhoneNumber { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string District { get; set; } // ตำบล

        public string Subdistrict { get; set; } // อำเภอ

        public string Province { get; set; }

        public string PostalCode { get; set; }

        public bool? IsDefault { get; set; }

        public DateTime? CreatedAt { get; set; }

        // Full address for display
        public string FullAddress => $"{AddressLine1} {AddressLine2} {District} {Subdistrict} {Province} {PostalCode}".Trim();
    }
}
