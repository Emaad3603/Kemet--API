using Kemet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Specifications.TravelAgencyPlanSpecs
{
    public class TravelAgencyPlanSpecifications : BaseSpecifications<TravelAgencyPlan>
    {
        public TravelAgencyPlanSpecifications():base()
        {
            Includes.Add(TA => TA.TravelAgency);
            Includes.Add(TA => TA.Price);
        }

    }
}
