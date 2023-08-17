using System.ComponentModel.DataAnnotations;

namespace CMS.DATA.Entities
{
    public class QuizOption : BaseEntity
    {
        public string QuizId { get; set; }
        public Quiz Quiz { get; set; }

        [MaxLength(150)]
        public List<string> Option { get; set; }
    }
}