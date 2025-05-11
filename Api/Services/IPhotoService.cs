namespace FOMSOData.Services
{
    public interface IPhotoService
    {
        Task<string?> UploadPhotoAsync(IFormFile file, string folder);
    }
}
