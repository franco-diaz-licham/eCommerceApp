namespace Backend.Src.Application.Services;

public class PhotoService : IPhotoService
{
    private readonly IMediaStorageService _cloudinaryService;
    private readonly DataContext _db;
    private readonly IMapper _mapper;

    public PhotoService(DataContext db, IMapper mapper, IMediaStorageService cloudinaryService)
    {
        _db = db;
        _mapper = mapper;
        _cloudinaryService = cloudinaryService;
    }

    /// <summary>
    /// Method which fetches a photo from the database.
    /// </summary>
    public async Task<PhotoDto?> GetAsync(int id)
    {
        var model = await _db.Photos.FindAsync(id);
        var output = _mapper.Map<PhotoDto>(model);
        return output;
    }

    /// <summary>
    /// Method which creates a photo and uploads to cloudinary.
    /// </summary>
    public async Task<PhotoDto> CreateImageAsync(PhotoCreateDto dto)
    {
        // create image
        if (dto.Image is null) throw new Exception("Photo is empty...");
        var transform = new Transformation().Height(800).Width(500).Crop("fill").Gravity("face");
        var imageResult = await _cloudinaryService.UploadPhotoAsync(dto.Image, transform);

        if (imageResult.Error != null) throw new Exception("Unable to save photo to cloud...");

        // map and save
        var newDto = new PhotoDto
        {
            FileName = dto.Image.FileName,
            PublicUrl = imageResult.SecureUrl.ToString(),
            PublicId = imageResult.PublicId
        };
        var model = _mapper.Map<PhotoEntity>(newDto);

        // save and return
        _db.Photos.Add(model);
        await _db.SaveChangesAsync();
        return _mapper.Map<PhotoDto>(model);
    }

    /// <summary>
    /// Method which deletes a photo and deletes it from cloudinary.
    /// </summary>
    public async Task<bool> DeleteAsync(PhotoDto dto)
    {
        var entity = await _db.Photos.FindAsync(dto.Id);
        if (entity is not null) _db.Photos.Where(e => e.Id == dto.Id).ExecuteDelete();
        await _db.SaveChangesAsync();

        try
        {
            await _cloudinaryService.DeletePhotoAsync(dto.PublicId);
        }
        catch
        {
            return false;
        }
        return true;
    }

    public async Task<bool> TryDeleteCloudAsync(string? publicId)
    {
        if (string.IsNullOrWhiteSpace(publicId)) return false;
        try 
        { 
            await _cloudinaryService.DeletePhotoAsync(publicId); 
        } 
        catch 
        {
            return false;
        }
        return true;
    }
}
