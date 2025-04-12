using Kemet.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.Images
{
    public class TravelAgencyImages : BaseEntity
    {
        public string ImageURl {  get; set; }

        public string TravelAgencyID { get; set; }

        public TravelAgency TravelAgency { get; set; }
    }
}
