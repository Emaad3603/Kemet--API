using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities
{
    public class Location :BaseEntity
    {
        public string Address { get; set; }
        public string LocationLink { get; set; }
        public string PlaceLatitude { get; set; }
        public string PlaceLongitude { get; set; }

      
    }
}
