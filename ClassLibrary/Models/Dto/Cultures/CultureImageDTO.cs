namespace ClassLibrary.Models.Dto.Cultures;

public class CultureImageDTO
{
    public int CultureImageId { get; set; }
    public int CultureId { get; set; }
    public string ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime? CreatedAt { get; set; }
}
