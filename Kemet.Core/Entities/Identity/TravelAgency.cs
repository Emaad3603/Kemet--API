using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.Identity
{
    public class TravelAgency : AppUser
    {

        public string Address { get; set; }

        public string Description { get; set; }

        public string TaxNumber { get; set; }

        public string FacebookURL { get; set; }

        public string InstagramURL { get; set; }
        //navigation prop for travel agency plans
        //each travel agency have many plans
      //  public List<TravelAgencyPlan> TravelAgencyPlan { get; set; }

    }
}
