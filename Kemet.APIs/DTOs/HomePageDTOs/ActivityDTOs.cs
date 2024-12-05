using Kemet.Core.Entities;
using Kemet.Core.Entities.Images;

namespace Kemet.APIs.DTOs.HomePageDTOs
{
    public class ActivityDTOs
    {
        public string Name { get; set; }
        public string Duration { get; set; }

        public List<string> imageURLs { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public double AverageRating { get; set; }
        public int? RatingsCount { get; set; } 
    }
}
