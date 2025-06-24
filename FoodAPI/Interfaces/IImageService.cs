using CloudinaryDotNet.Actions;

namespace FoodAPI.Interfaces
{
    public interface IImageService
    {
        Task<ImageUploadResult> UploadProfileImageAsync(IFormFile file);
        Task<ImageUploadResult> UploadBannerImageAsync(IFormFile file);
        Task<DeletionResult> DeleteImageAsync(string publicId);
    }
}
