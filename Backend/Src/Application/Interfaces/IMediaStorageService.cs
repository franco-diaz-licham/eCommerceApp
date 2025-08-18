namespace Backend.Src.Application.Interfaces;

public interface IMediaStorageService
{
    Task<DeletionResult> DeletePhotoAsync(string publicId);
    Task<ImageUploadResult> UploadPhotoAsync(byte[] fileContent, string fileName, Transformation transform);
    Task<ImageUploadResult> UploadPhotoAsync(IFormFile file, Transformation transform);
}