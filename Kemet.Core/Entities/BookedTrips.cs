using Kemet.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities
{
    public class BookedTrips : BaseEntity
    {
        public int TrabelAgencyPlanID { get; set; }

        public TravelAgencyPlan travelAgencyPlan { get; set; }

        public string CustomerID { get; set; }

        public Customer Customer { get; set; }

        public string TravelAgencyName { get; set; }

        public int NumOfPeople { get; set; }

        public string ReserveType { get; set; }

        public DateOnly ReserveDate { get; set;}

        public string BookedCategory { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // time of the created review
    }
}
