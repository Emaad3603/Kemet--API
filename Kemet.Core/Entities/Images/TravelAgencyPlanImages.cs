using Kemet.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.Images
{
     public class TravelAgencyPlanImages : BaseEntity
    {
        public string ImageURl { get; set; }

        public int TravelAgencyPlanID { get; set; }

        public TravelAgencyPlan TravelAgencyPlan { get; set; }
    }
}
