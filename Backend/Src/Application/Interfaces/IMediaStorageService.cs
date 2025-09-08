namespace Backend.Src.Application.Interfaces;

public interface IMediaStorageService
{
    /// <summary>
    /// Method which deletes photos from cloudinary.
    /// </summary>
    Task<DeletionResult> DeletePhotoAsync(string publicId);

    /// <summary>
    /// Method which uploads images to cloudinary.
    /// </summary>
    Task<ImageUploadResult> UploadPhotoAsync(byte[] fileContent, string fileName, Transformation transform);

    /// <summary>
    /// Method which uploads images to cloudinary.
    /// </summary>
    Task<ImageUploadResult> UploadPhotoAsync(IFormFile file, Transformation transform);
}