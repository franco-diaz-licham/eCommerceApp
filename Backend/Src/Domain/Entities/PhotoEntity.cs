namespace Backend.Src.Domain.Entities;

public class PhotoEntity : BaseEntity
{
    public PhotoEntity() { }
    public PhotoEntity(string fileName, string publicId, string publicUrl)
    {
        FileName = fileName;
        PublicId = publicId;
        PublicUrl = publicUrl;
    }

    public string FileName { get; set; } = default!;
    public string PublicUrl { get; set; } = default!;
    public string PublicId { get; set; } = default!;
    public DateTime CreatedOn { get; set; }
}
