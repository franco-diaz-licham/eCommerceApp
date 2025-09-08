namespace Backend.Src.Infrastructure.Services;

public class CloudinaryService : IMediaStorageService
{
    private readonly Cloudinary _cloud;

    public CloudinaryService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
        _cloud = new Cloudinary(acc);
    }

    public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile file, Transformation transform)
    {
        if (file.Length == 0) return new();
        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = transform
        };

        return await _cloud.UploadAsync(uploadParams);
    }

    public async Task<ImageUploadResult> UploadPhotoAsync(byte[] fileContent, string fileName, Transformation transform)
    {
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(fileName, new MemoryStream(fileContent)),
            Transformation = transform
        };

        return await _cloud.UploadAsync(uploadParams);
    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloud.DestroyAsync(deleteParams);
        return result;
    }
}

public class CloudinarySettings
{
    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
}
