using Kemet.Core.Entities.WishlistEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.WishlistEntites
{
    public class WishlistPlaces : BaseEntity
    {
        public int WishlistID { get; set; }

        


        public int PlaceID { get; set; }

        public Place Place { get; set; }


    }
}
