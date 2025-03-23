using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.ModelView
{
    public class TravelAgencySearchDTO
    {

        public string UserName { get; set; }
         
        public string? pictureUrl { get; set; }

        public string? Description { get; set; }

        public string? Bio { get; set; }
    }
}
