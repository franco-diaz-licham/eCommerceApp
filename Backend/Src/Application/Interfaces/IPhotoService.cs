namespace Backend.Src.Application.Interfaces;

public interface IPhotoService
{
    Task<PhotoDTO> CreateImageAsync(PhotoCreateDTO dto);
    Task<bool> DeleteAsync(PhotoDTO dto);
    Task<PhotoDTO?> GetAsync(int id);
    Task<bool> TryDeleteCloudAsync(string? publicId);
}