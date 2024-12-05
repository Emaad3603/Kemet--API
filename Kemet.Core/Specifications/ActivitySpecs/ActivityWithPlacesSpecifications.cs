using Kemet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Specifications.ActivitySpecs
{
    public class ActivityWithPlacesSpecifications : BaseSpecifications<Activity>
    {
        public ActivityWithPlacesSpecifications() :base()
        {
            Includes.Add(A => A.Place);
            Includes.Add(A => A.Images);
            Includes.Add(A=>A.Reviews);
        }
    }
}
