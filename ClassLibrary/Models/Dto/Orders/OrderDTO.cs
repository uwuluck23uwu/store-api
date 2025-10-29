#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Dto
{
    public class OrderDTO
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public string OrderNumber { get; set; }

        public decimal? TotalAmount { get; set; }

        public string Status { get; set; } // Pending, Confirmed, Preparing, Shipping, Delivered, Cancelled

        public string PaymentStatus { get; set; } // Pending, Paid, Refunded

        public string Notes { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int ItemCount { get; set; }

        // Order Items for list view
        public List<OrderItemSummaryDTO> OrderItems { get; set; }
    }

    public class OrderItemSummaryDTO
    {
        public int OrderItemId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductImageUrl { get; set; }

        public int? Quantity { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? TotalPrice { get; set; }

        public string Unit { get; set; }
    }
}
