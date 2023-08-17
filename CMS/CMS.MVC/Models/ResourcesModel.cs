using CMS.DATA.Enum;
using System.ComponentModel.DataAnnotations;

namespace CMS.MVC.Models
{
    public class ResourcesModel
    {
        public string CourseId { get; set; }
        public string AddedById { get; set; }
        public Modules Module { get; set; }
        public ModuleWeeks Weeks { get; set; }
        public IFormFile VideoFile { get; set; }

        [MaxLength(150)]
        public string Topic { get; set; }

        public string Text { get; set; }
        public string VideoUrl { get; set; }
        public string PublicId { get; set; }
        public bool CompletionStatus { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public List<UploadRecord> uploadRecords { get; set; }
    }
}
