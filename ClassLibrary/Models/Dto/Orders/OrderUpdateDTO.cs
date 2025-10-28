#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models.Dto
{
    public class OrderUpdateDTO
    {
        [MaxLength(1000)]
        public string Notes { get; set; }
    }
}
