using Kemet.APIs.DTOs.ReviewsDTOs;
using Kemet.APIs.Helpers;
using Kemet.Core.Entities;
using System.Text.Json.Serialization;

namespace Kemet.APIs.DTOs.DetailedDTOs
{
    public class DetailedActivityDTOs
    {
        public int ActivityId { get; set; }
        public string Name { get; set; }
        public string Duration { get; set; }

        public string CulturalTips { get; set; }

        public string Description { get; set; }

        public string CategoryName { get; set; }
        public TimeSpan OpenTime { get; set; }

        public TimeSpan CloseTime { get; set; }
        public int GroupSize { get; set; }

        public List<string> imageURLs { get; set; }
        public ICollection<Review?> Reviews { get; set; }
        public double AverageRating { get; set; }
        public int? RatingsCount { get; set; }

        public decimal? EgyptianAdult { get; set; }

        public decimal? EgyptianStudent { get; set; }

        public decimal? TouristAdult { get; set; }

        public decimal? TouristStudent { get; set; }

        public string Address { get; set; }


        public string? LocationLink { get; set; }


        public string? PlaceLatitude { get; set; }


        public string? PlaceLongitude { get; set; }
    }
}
