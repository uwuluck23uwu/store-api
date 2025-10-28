#nullable disable
using System;
using System.Collections.Generic;

namespace ClassLibrary.Models.Dto
{
    public class CartDTO
    {
        public int UserId { get; set; }

        public List<CartItemDTO> CartItems { get; set; }

        public decimal TotalPrice { get; set; }

        public int TotalItems { get; set; }
    }
}
