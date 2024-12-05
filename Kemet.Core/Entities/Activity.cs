using Kemet.APIs.Helpers;
using Kemet.Core.Entities.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kemet.Core.Entities
{
    public class Activity :BaseEntity
    {
        public string Name { get; set; }
        public string Duration { get; set; }

        public string CulturalTips { get; set; }

        public string Description { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        // Price relation nav 
        public int? priceId { get; set; }
        public Price Price { get; set; }
        [JsonConverter(typeof(CustomTimeSpanConverter))]  // Apply custom converter for TimeSpan
        public TimeSpan OpenTime { get; set; }

        [JsonConverter(typeof(CustomTimeSpanConverter))]  // Apply custom converter for TimeSpan
        public TimeSpan CloseTime { get; set; }
        public int GroupSize { get; set; }
        public string? PictureUrl { get; set; }

        //has one location
        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        //one place has many activities
        public int? PlaceId { get; set; }
        public Place? Place { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<ActivityImage> Images { get; set; } = new List<ActivityImage>();
    }
}
