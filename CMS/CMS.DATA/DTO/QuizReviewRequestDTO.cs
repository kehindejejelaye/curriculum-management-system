using CMS.DATA.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DATA.DTO
{
    public class QuizReviewRequestDTO
    {
        [Required]

        public string CourseId { get; set; }

        [Required]
        public string UserId { get; set; }
        
        public DateTime Timestamp { get; set; }
        public string Notes { get; set; }

        [Required]
        public bool IsSatisfied { get; set; }

    }
}
