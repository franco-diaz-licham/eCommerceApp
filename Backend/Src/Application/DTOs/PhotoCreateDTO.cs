namespace Backend.Src.Application.DTOs;

public class PhotoCreateDTO
{
    public string FileName { get; set; } = string.Empty;
    public string PublicUrl { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;
    public IFormFile? Image { get; set; }
}
