#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Dto
{
    public class OrderDTO
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public int AddressId { get; set; }

        public string OrderNumber { get; set; }

        public decimal? TotalAmount { get; set; }

        public decimal? ShippingFee { get; set; }

        public string Status { get; set; } // Pending, Confirmed, Preparing, Shipping, Delivered, Cancelled

        public string PaymentStatus { get; set; } // Pending, Paid, Refunded

        public string Notes { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int ItemCount { get; set; }
    }
}
