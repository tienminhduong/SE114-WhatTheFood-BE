using CloudinaryDotNet.Actions;

namespace FoodAPI.Interfaces
{
    public interface IImageService
    {
        Task<ImageUploadResult> AddImageAsync(IFormFile file, int width, int height);
        Task<DeletionResult> DeleteImageAsync(string publicId);
    }
}
