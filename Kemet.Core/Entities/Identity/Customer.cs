using Kemet.Core.Entities.Intersts;
using Kemet.Core.Entities.WishlistEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.Identity
{
    public class Customer :AppUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string SSN { get; set; }

        public string Gender { get; set; }

        public string Nationality { get; set; }

        public ICollection<CustomerInterest> CustomerInterests { get; set; } = new List<CustomerInterest>();

        public string? Bio { get; set; }

        public string? Country { get; set; }

        public string? City { get; set; }

        public string? WebsiteLink { get; set; }

        public int? WishlistID { get; set; }

        public Wishlist? Wishlist { get; set; }

        public ICollection<BookedTrips> BookedTrips { get; set; } = new List<BookedTrips>();
    }
}
