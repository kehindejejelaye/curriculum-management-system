using CMS.DATA.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DATA.DTO
{
    public class QuizSummaryDto
    {
        public List<LessonQuizesSummaryDto>  QuizesSummary { get; set; }

        public double Percentage { get; set; }

        public int CorrectCount { get; set; }

        public int IncorrectCount { get; set; }
        public int SkippedCount { get; set; }

        public List<ModuleWeeks> Weeks { get; set; }


    }
}
