using Kemet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Specifications.PlaceSpecs
{
    public class PlaceWithCategoriesAndactivitiesSpecifications : BaseSpecifications<Place>
    {
        //this constructor will be used for creating object for get all places
        public PlaceWithCategoriesAndactivitiesSpecifications() :base()
        {
        
            Includes.Add(P => P.Category);
            Includes.Add(P => P.Images);
        }
    }
}
