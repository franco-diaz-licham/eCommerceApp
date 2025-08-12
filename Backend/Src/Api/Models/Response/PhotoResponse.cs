namespace Backend.Src.Api.Models.Response;

public class PhotoResponse
{
    public int Id { get; set; }
    public string PublicUrl { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;
}
