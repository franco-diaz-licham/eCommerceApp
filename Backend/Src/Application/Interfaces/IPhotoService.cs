namespace Backend.Src.Application.Interfaces;

public interface IPhotoService
{
    /// <summary>
    /// Method which creates a photo and uploads to cloudinary.
    /// </summary>
    Task<PhotoDto> CreateImageAsync(PhotoCreateDto dto);

    /// <summary>
    /// Method which deletes a photo and deletes it from cloudinary.
    /// </summary>
    Task<bool> DeleteAsync(PhotoDto dto);

    /// <summary>
    /// Method which fetches a photo from the database.
    /// </summary>
    Task<PhotoDto?> GetAsync(int id);

    /// <summary>
    /// Attempted to delete cloudinary photo.
    /// </summary>
    Task<bool> TryDeleteCloudAsync(string? publicId);
}