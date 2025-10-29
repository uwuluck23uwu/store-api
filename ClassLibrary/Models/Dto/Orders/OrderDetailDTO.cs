#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Dto
{
    public class OrderDetailDTO
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public string OrderNumber { get; set; }

        public decimal? TotalAmount { get; set; }

        public string Status { get; set; }

        public string PaymentStatus { get; set; }

        public string Notes { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // User Info
        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string UserPhone { get; set; }

        // Order Items
        public List<OrderItemDetailDTO> OrderItems { get; set; }

        // Payment Info
        public PaymentDTO Payment { get; set; }
    }

    public class OrderItemDetailDTO
    {
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductImageUrl { get; set; }

        public int SellerId { get; set; }

        public string ShopName { get; set; }

        public int? Quantity { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? TotalPrice { get; set; }

        public decimal? Subtotal { get; set; }

        public string Unit { get; set; }

        // Navigation properties for detailed view
        public ProductDTO Product { get; set; }

        public SellerDTO Seller { get; set; }

        public OrderDTO Order { get; set; }
    }

    public class PaymentDTO
    {
        public int PaymentId { get; set; }

        public string Method { get; set; }

        public decimal? Amount { get; set; }

        public string Status { get; set; }

        public string ReferenceCode { get; set; }

        public DateTime? PaidAt { get; set; }
    }
}
