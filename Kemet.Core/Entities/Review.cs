using Kemet.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities
{
    public class Review :BaseEntity
    {
        public string UserId { get; set; }  // ID of the user who wrote the review

        public string USERNAME { get; set; }
        public DateOnly Date { get; set; }
        public string ReviewTitle { get; set; }
        public string VisitorType { get; set; }
        public string UserImageURl {  get; set; }
        public string Comment { get; set; } // The review text
        public int Rating { get; set; } // Rating, e.g., 1-5

        public string? ImageUrl { get; set; }

        //Foreign keys to link different entities
        public int? ActivityId { get; set; }
        public Activity? Activity { get; set; }


        public int? PlaceId { get; set; }
        public Place? Place { get; set; }

        public int? TravelAgencyPlanId { get; set; }
        public TravelAgencyPlan? TravelAgencyPlan { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // time of the created review



    }
}
