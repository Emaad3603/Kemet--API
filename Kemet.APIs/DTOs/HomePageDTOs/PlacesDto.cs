using Kemet.Core.Entities;
using Kemet.Core.Entities.Images;

namespace Kemet.APIs.DTOs.HomePageDTOs
{
    public class PlacesDto
    {
        public int PlaceID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
     
        public List<string> ImageURLs { get; set;}

        public ICollection<Review> Reviews { get; set; }
        public double AverageRating { get; set; }
        public int? RatingsCount { get; set; }

    }
}
