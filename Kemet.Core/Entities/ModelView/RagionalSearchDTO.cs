using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Entities.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Services
{
    public class RagionalSearchDTO
    {
        public ICollection<Place> Places { get; set; }
        public ICollection<Activity> Activities { get; set; }
        public ICollection<TravelAgencyPlan> TravelAgencyPlans { get; set; }

        public ICollection<TravelAgencySearchDTO> TravelAgencies { get; set; }
    }
}
