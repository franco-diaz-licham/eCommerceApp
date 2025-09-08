namespace Backend.Src.Application.DTOs;

public class PhotoCreateDto
{
    public PhotoCreateDto(IFormFile image)
    {
        Image = image;
    }

    public IFormFile? Image { get; set; }
}
