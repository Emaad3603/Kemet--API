using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.Core.Entities;
using Kemet.Core.Entities.WishlistEntites;

namespace Kemet.APIs.DTOs.WishlistDtos
{
    public class WishlistDto
    {

       // public string UserID { get; set; }

        public List<PlacesDto?> Places { get; set; }

        public List<ActivityDTOs?> Activities { get; set; }

        public List<TravelAgencyPlanDTOs?> Plans { get; set; }
    }
}
