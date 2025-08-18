namespace Backend.Src.Application.Interfaces;

public interface IPhotoService
{
    Task<PhotoDto> CreateImageAsync(PhotoCreateDto dto);
    Task<bool> DeleteAsync(PhotoDto dto);
    Task<PhotoDto?> GetAsync(int id);
    Task<bool> TryDeleteCloudAsync(string? publicId);
}