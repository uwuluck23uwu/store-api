using Microsoft.AspNetCore.Http;

namespace ClassLibrary.Models.Dto.Cultures;

public class CultureUpdateDTO
{
    public string? CultureName { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public IFormFile? Image { get; set; } // For backward compatibility
    public List<IFormFile>? Images { get; set; } // Multiple images support
    public List<int>? DeletedImageIds { get; set; } // IDs of images to delete
    public bool? IsActive { get; set; }
}
