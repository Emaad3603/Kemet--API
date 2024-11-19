using Kemet.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities
{
    public class TravelAgencyPlan:BaseEntity
    {
        public string PlanName { get; set; }
        public int Duration { get; set; }

        // Price relation nav 
        public int? priceId { get; set; }
        public Price Price { get; set; }
        public string Description { get; set; }
        public string PlanAvailability { get; set; }
        public string PictureUrl { get; set; }

        //navigation prop of travel agency and FK
        public string TravelAgencyId { get; set; } //FK
        public TravelAgency TravelAgency { get; set; }


    }
}
