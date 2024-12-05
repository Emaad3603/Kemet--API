using Kemet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Specifications.ReviewSpecs
{
    public class TravelAgencyPlansWithReviewsSpecifications :BaseSpecifications<TravelAgencyPlan>
    {
        public TravelAgencyPlansWithReviewsSpecifications(int travelAgencyPlanId):base(TA=>TA.Id==travelAgencyPlanId)
        {
            Includes.Add(TA => TA.Reviews);
        }
    }
}
