﻿using Kemet.Core.Entities;

namespace Kemet.APIs.DTOs.DetailedDTOs
{
    public class B7B7ActivityDto
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

        public string imageURL { get; set; }
      
        public decimal? EgyptianAdult { get; set; }       

        public decimal? TouristAdult { get; set; }
      
        public string Address { get; set; }

        public string? LocationLink { get; set; }

    }
}
