using Kemet.Core.Entities;

namespace Kemet.APIs.DTOs.HomePageDTOs
{
    public class AddActivityDto
    {

        public int ActivityId { get; set; }
        public string Name { get; set; }
        public string Duration { get; set; }

        public string? PictureUrl { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string CultureTips { get; set; }
        public decimal EgyptianAdultCost { get; set; }
        public decimal EgyptianStudentCost { get; set; }
        public decimal TouristAdultCost { get; set; }
        public decimal TouristStudentCost { get; set; }
        public List<string> ImageURLs { get; set; }
    }
}
