namespace Backend.Src.Api.Models.Request;

public class PhotoCreateRequest
{
    public string FileName { get; set; } = string.Empty;
    [Required] public IFormFile? Image { get; set; }
}
