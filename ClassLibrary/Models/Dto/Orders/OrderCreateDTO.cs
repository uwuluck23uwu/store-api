#nullable disable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class OrderCreateDTO
    {
        [Required(ErrorMessage = "Address ID is required")]
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Order items are required")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item")]
        public List<OrderItemCreateDTO> Items { get; set; }

        [MaxLength(1000)]
        public string Note { get; set; }

        public decimal ShippingFee { get; set; } = 0;
    }

    // Alias for backward compatibility
    public class OrderItemCreateDTO
    {
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    // Used by AutoMapper
    public class CreateOrderItemDTO
    {
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
