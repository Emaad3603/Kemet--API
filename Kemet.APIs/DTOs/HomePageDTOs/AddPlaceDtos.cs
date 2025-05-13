using Kemet.Core.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Kemet.APIs.DTOs.HomePageDTOs
{
    public class AddPlaceDtos
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string CategoryName { get; set; }

        public string CulturalTips { get; set; }

        public string Duration { get; set; }

        public decimal EgyptianAdultCost { get; set; }

        public decimal EgyptianStudentCost { get; set; }

        public decimal TouristAdultCost { get; set; }

        public decimal TouristStudentCost { get; set; }

        public List<IFormFile?> ImageURLs { get; set; }

        public TimeSpan OpenTime { get; set; }

        public TimeSpan CloseTime { get; set; }
    }
}
