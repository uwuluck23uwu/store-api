#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Data;

public partial class Location
{
    public int Id { get; set; }

    public string LocationId { get; set; } = string.Empty; // LO-XXXX (Running number)

    public string RefId { get; set; } // Reference ID (optional)

    // ข้อมูลพื้นฐาน
    public string LocationName { get; set; } = string.Empty;

    public string Description { get; set; }

    public string LocationType { get; set; } = "Store"; // Store, Pickup, Farm, Tourist, etc.

    // พิกัด GPS
    public decimal Latitude { get; set; } // เช่น 13.7563

    public decimal Longitude { get; set; } // เช่น 100.5018

    // ข้อมูลเพิ่มเติม
    public string Address { get; set; }

    public string PhoneNumber { get; set; }

    public string ImageUrl { get; set; }

    public string IconUrl { get; set; } // ไอคอนของหมุด

    public string IconColor { get; set; } // สีของหมุด เช่น "green", "brown"

    // ความสัมพันธ์กับ Seller (Optional)
    public int? SellerId { get; set; }

    public virtual Seller Seller { get; set; }

    // ความสัมพันธ์กับ Products (Optional - สินค้าที่ขายในจุดนี้)
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    // สถานะ
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
