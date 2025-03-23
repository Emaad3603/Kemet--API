using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kemet.Core.Entities.AI_Entites
{
   

    public class AiRequestDto
    {
        [Required]
        public AnswersDto Answers { get; set; }
    }

    public class AnswersDto
    {
        public List<string> ExperiencesTypes { get; set; }
        public int NumberOfDays { get; set; }
        public List<string> Places { get; set; }
        public List<string> Activity { get; set; }
        public string Season { get; set; }
        public string Budget { get; set; }
    }

}
