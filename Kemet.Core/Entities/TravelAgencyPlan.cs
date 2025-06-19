using Kemet.Core.Entities.Identity;
using Kemet.Core.Entities.Images;
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
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        // Price relation nav 
        public int? priceId { get; set; }
        public Price Price { get; set; }
        public string Description { get; set; }
        public string? PlanAvailability { get; set; }
        public string PictureUrl { get; set; }

        //navigation prop of travel agency and FK
        public string TravelAgencyId { get; set; } //FK
        public TravelAgency TravelAgency { get; set; }

        public double AverageRating { get; set; } = 0.0;  // Default to 0
        public int RatingsCount { get; set; } = 0;       // Default to 0

        public string? PlanLocation { get; set; }

        public decimal? HalfBoardPriceAddittion {  get; set; }     

        public decimal? FullBoardPriceAddition { get; set; }

        public ICollection<TravelAgencyPlanImages> images { get; set; } = new List<TravelAgencyPlanImages>();

    }
}
