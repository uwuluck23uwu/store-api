#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Data;

public partial class Address
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

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
