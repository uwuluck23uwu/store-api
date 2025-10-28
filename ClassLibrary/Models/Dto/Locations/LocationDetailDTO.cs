#nullable disable
using System.Collections.Generic;

namespace ClassLibrary.Models.Dto
{
    public class LocationDetailDTO : LocationDTO
    {
        public SellerDTO SellerInfo { get; set; }
        public List<ProductListDTO> Products { get; set; }
    }
}
