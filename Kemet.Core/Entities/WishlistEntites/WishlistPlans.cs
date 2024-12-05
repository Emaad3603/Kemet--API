using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.WishlistEntites
{
    public class WishlistPlans : BaseEntity
    {
        public int WishlistID { get; set; }

        


        public int TravelAgencyPlanID { get; set; }

        public TravelAgencyPlan TravelAgencyPlan { get; set; }
    }
}
