using Kemet.Core.Entities;

namespace Kemet.APIs.DTOs.DetailedDTOs
{
    public class DetailedTravelAgencyPlanDto
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public string? PlanAvailability { get; set; }

        public  string? planLocation { get; set; }
        public string Duration { get; set; }

        public string Description { get; set; }

        public string imageURLs { get; set; }
        public ICollection<Review?> Reviews { get; set; }
        public double AverageRating { get; set; }
        public int? RatingsCount { get; set; }

        public decimal? EgyptianAdult { get; set; }

        public decimal? EgyptianStudent { get; set; }

        public decimal? TouristAdult { get; set; }

        public decimal? TouristStudent { get; set; }

        public string TravelAgencyName { get; set; }

        public string TravelAgencyDescription { get; set; }

        public string TravelAgencyAddress { get; set; }



     


    }
}
