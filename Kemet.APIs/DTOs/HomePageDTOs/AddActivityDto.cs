using Kemet.Core.Entities;

namespace Kemet.APIs.DTOs.HomePageDTOs
{
    public class AddActivityDto
    {

        
        public string Name { get; set; }
        public string Duration { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string CulturalTips { get; set; }
        public decimal EgyptianAdultCost { get; set; }
        public decimal EgyptianStudentCost { get; set; }
        public decimal TouristAdultCost { get; set; }
        public decimal TouristStudentCost { get; set; }
        public List<IFormFile?> ImageURLs { get; set; }

        public TimeSpan OpenTime { get; set; }
        
        public TimeSpan CloseTime { get; set; }

        public string Address { get; set; }
        public string? LocationLink { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }
}
