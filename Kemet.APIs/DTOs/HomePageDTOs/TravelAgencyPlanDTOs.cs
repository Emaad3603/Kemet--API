using Kemet.Core.Entities;

namespace Kemet.APIs.DTOs.HomePageDTOs
{
    public class TravelAgencyPlanDTOs
    {
        public string Name { get; set; }
        public string PlanName { get; set; }
        public string PlanAvailability { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }

        public decimal? EgyptianAdult { get; set; }

        public decimal? EgyptianStudent { get; set; }

        public decimal? TouristAdult { get; set; }

        public decimal? TouristStudent { get; set; }
    }
}
