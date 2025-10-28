#nullable disable
using System;

namespace ClassLibrary.Models.Dto
{
    public class OrderItemDTO
    {
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ImageUrl { get; set; }

        public int SellerId { get; set; }

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? Subtotal { get; set; }

        public string Unit { get; set; }
    }
}
