using Kemet.Core.Entities.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.WishlistEntites
{
    public class Wishlist : BaseEntity
    {
        public string UserID { get; set; }

        public List<WishlistPlaces?> Places { get; set; } = new List<WishlistPlaces>();

        public List<WishlistActivites?> Activities { get; set; } = new List<WishlistActivites>();

        public List<WishlistPlans?> Plans { get; set; } = new List<WishlistPlans>();
    }
}
