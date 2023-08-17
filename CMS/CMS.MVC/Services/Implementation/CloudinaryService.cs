using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using CMS.MVC.Services.ServicesInterface;

namespace CMS.MVC.Services.Implementation
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly IConfiguration _configuration;

        public CloudinaryService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<VideoUploadResult> UploadVideo(IFormFile file, object id)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("No Video Uploaded");
            }

            var cloudinary = new Cloudinary(new Account(_configuration["Cloudinary:CloudName"], _configuration["Cloudinary:ApiKey"], _configuration["Cloudinary:ApiSecret"]));
            using var stream = file.OpenReadStream();
            var uploadParams = new VideoUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = $"{id}"
            };

            var result = await cloudinary.UploadAsync(uploadParams);
            if (result != null)
            {
                return result;
            }

            throw new Exception("Video Failed to Upload");
        }

    }
}
