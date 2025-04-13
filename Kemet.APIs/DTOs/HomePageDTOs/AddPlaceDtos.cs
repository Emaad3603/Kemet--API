using Kemet.Core.Entities;

namespace Kemet.APIs.DTOs.HomePageDTOs
{
    public class AddPlaceDtos
    {
        public int PlaceID { get; set; }
    
      //  public double AverageRating { get; set; }
       // public int? RatingsCount { get; set; }

            public string Name { get; set; }
            public string Description { get; set; }
            public string CategoryName { get; set; }
        public string CultureTips { get; set; }
        public string Duration { get; set; }
      //  public int? priceId { get; set; }
        public decimal EgyptianAdultCost { get; set; }
        public decimal EgyptianStudentCost { get; set; }
        public decimal TouristAdultCost { get; set; }
        public decimal TouristStudentCost { get; set; }
        public List<IFormFile> ImageURLs { get; set; }


      

    }
}
