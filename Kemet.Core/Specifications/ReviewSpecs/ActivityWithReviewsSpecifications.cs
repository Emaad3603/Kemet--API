using Kemet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Specifications.ReviewSpecs
{
    public class ActivityWithReviewsSpecifications : BaseSpecifications<Activity>
    {
        public ActivityWithReviewsSpecifications(/*int activityId*/) :base(/*a=>a.Id==activityId*/)
        {
        
        Includes.Add(a=>a.Reviews);
        
        }
    }
}
