using Kemet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Specifications.ReviewSpecs
{
    public class PlaceWithReviewsSpecifications :BaseSpecifications<Place>
    {
        public PlaceWithReviewsSpecifications(int placeId):base(p=>p.Id==placeId)
        {
            Includes.Add(p => p.Reviews);
        }
    }
}
