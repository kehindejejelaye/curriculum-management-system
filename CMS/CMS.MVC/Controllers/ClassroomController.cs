using CMS.DATA.DTO;
using CMS.DATA.Entities;
using CMS.MVC.Models;
using CMS.MVC.Services.ServicesInterface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CMS.MVC.Controllers
{
    public class ClassroomController : Controller
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IConfiguration _configuration;

        public ClassroomController(ICloudinaryService cloudinaryService, IConfiguration configuration)
        {
            _cloudinaryService = cloudinaryService;
            _configuration = configuration;
        }
        public bool toggleState { get; set; } = false;

        [HttpGet]
        public IActionResult LearningContent()
        {
            ViewBag.ShowSuccessModal = toggleState;

            return View();
        }

        [HttpGet]
        public IActionResult QuizPage()
        {
            ViewBag.ShowSuccessModal = toggleState;

            return View();
        }
        public async Task<IActionResult> ResourcePage()
        {
            var result = new ResourcesModel();
            using (var client = new HttpClient())
            {
                var apiUrl = _configuration["baseUrl:localhost"] + ConstantSubBaseEnpoint.GetLessonUri;
                var response = await client.GetFromJsonAsync<ResponseDto<IEnumerable<Lesson>>>(apiUrl);
                var uploadResults = new List<UploadRecord>();
                foreach (var userResult in response.Result)
                {
                    var uploadResult = new UploadRecord();
                    uploadResult.DocName = userResult.Topic;
                    uploadResult.CreatedDate = userResult.DateCreated;
                    uploadResult.Id = userResult.Id;
                    uploadResults.Add(uploadResult);
                }
                

                result.uploadRecords = uploadResults;
            }
             return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> ResourcePage(ResourcesModel model)
        {
            try
            {
                var uploadVideo = await _cloudinaryService.UploadVideo(model.VideoFile, model.Module.ToString());
                if (uploadVideo.Url.ToString() != null)
                {
                    model.VideoUrl = uploadVideo.Url.ToString();
                    model.PublicId = uploadVideo.PublicId;
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //This will be replace with context of signIn User "36e318aa-6d02-46d9-8048-3e2a8182a6c3"
                model.AddedById = userId;
                using (var client = new HttpClient())
                {
                    var apiUrl = _configuration["baseUrl:localhost"] + ConstantSubBaseEnpoint.AddLessonUri;
                    var response = await client.PostAsJsonAsync(apiUrl, model);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("FacilitatorDashboard", "Dashboard");
                    }
                    else
                    {
                        return RedirectToAction("Error");
                    }
                }
            }
            catch (Exception ex)
            {
                return View(model);
                
            }



        }
        public IActionResult QuizScore()
        {
            return View();
        }
    }
}
