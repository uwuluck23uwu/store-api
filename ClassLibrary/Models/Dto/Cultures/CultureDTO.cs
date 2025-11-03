namespace ClassLibrary.Models.Dto.Cultures;

public class CultureDTO
{
    public int CultureId { get; set; }
    public string CultureName { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string Category { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<CultureImageDTO> CultureImages { get; set; }
}
