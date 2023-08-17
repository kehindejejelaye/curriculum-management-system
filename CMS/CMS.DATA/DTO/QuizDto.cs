using CMS.DATA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DATA.DTO
{
    public class QuizDto
    {
      
        public string Questiontext { get; set; }


        public string CorrectAnswer { get; set; }

        public string Instruction { get; set; }
      
      
    }
}
