namespace Backend.Src.Domain.Entities;

public class PhotoEntity : BaseEntity
{
    private PhotoEntity() { }
    public PhotoEntity(string fileName, string publicId, string publicUrl)
    {
        SetFileName(fileName);
        ReplaceRemote(publicId, publicUrl);
    }

    #region Properties
    public string FileName { get; private set; } = default!;
    public string PublicId { get; private set; } = default!;
    public string PublicUrl { get; private set; } = default!;
    #endregion

    #region Business Logic
    public void SetFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException($"{nameof(fileName)} is required.");
        var collapsed = CollapseSpaces(fileName.Trim());
        if (collapsed.Length > 128) throw new ArgumentException($"{nameof(fileName)} too long (max 64).");

        FileName = collapsed;
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
    }

    private static string CollapseSpaces(string s) => Regex.Replace(s, @"\s+", " ");
    #endregion
}
