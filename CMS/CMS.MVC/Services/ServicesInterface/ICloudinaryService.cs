using CloudinaryDotNet.Actions;

namespace CMS.MVC.Services.ServicesInterface
{
    public interface ICloudinaryService
    {
        Task<VideoUploadResult> UploadVideo(IFormFile file, object id);
    }
}
