namespace Backend.Src.Domain.Entities;

public class PhotoEntity : BaseEntity
{
    protected PhotoEntity() { }
    public PhotoEntity(string fileName, string publicId, string publicUrl)
    {
        SetFileName(fileName);
        ReplaceRemote(publicId, publicUrl);
        CreatedOn = DateTime.UtcNow;
    }

    #region Properties
    public string FileName { get; private set; } = default!;
    public string PublicId { get; private set; } = default!;
    public string PublicUrl { get; private set; } = default!;
    public DateTime CreatedOn { get; private set; }
    public DateTime? UpdatedOn { get; private set; }
    #endregion

    #region Business Logic
    public void SetFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException("Name is required.");
        var collapsed = CollapseSpaces(fileName.Trim());
        if (collapsed.Length > 128) throw new ArgumentException("Name too long (max 64).");

        FileName = collapsed;
        Touch();
    }

    public void ReplaceRemote(string newPublicId, string newPublicUrl)
    {
        // Validate public Id
        if (string.IsNullOrWhiteSpace(newPublicId)) throw new ArgumentException($"Public Id is required.");
        var publicId = CollapseSpaces(newPublicId.Trim());

        // Validate url
        if (!Uri.TryCreate(newPublicUrl, UriKind.Absolute, out var uri)) throw new ArgumentException("PublicUrl must be an absolute URL.");
        if (uri.Scheme != Uri.UriSchemeHttps) throw new ArgumentException("PublicUrl must use HTTPS.");

        PublicId = publicId;
        PublicUrl = newPublicUrl;
        Touch();
    }

    private static string CollapseSpaces(string s) => Regex.Replace(s, @"\s+", " ");
    private void Touch() => UpdatedOn = DateTime.UtcNow;
    #endregion
}
