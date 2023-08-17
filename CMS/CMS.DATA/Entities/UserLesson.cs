using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DATA.Entities
{
    public class UserLesson:BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string LessonId { get; set; }

        public Lesson Lesson { get; set; }

        public bool CompletionStatus { get; set; }
    }
}
