using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FoodAPI.Interfaces;

namespace FoodAPI.Services
{
    public class ImageService(IConfiguration config) : IImageService
    {
        private readonly Cloudinary cloudinary = new(new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            ));

        public async Task<DeletionResult> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            return await cloudinary.DestroyAsync(deleteParams);
        }

        public Task<ImageUploadResult> UploadBannerImageAsync(IFormFile file)
        {
            throw new NotImplementedException();
        }

        public async Task<ImageUploadResult> UploadProfileImageAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                AssetFolder = "onefourteenfood"
            };
            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            return uploadResult;
        }
    }
}
