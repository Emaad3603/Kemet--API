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
    public class Place: BaseEntity
    {
        public string Name { get; set; }
        public string CultureTips { get; set; }
        public string Description { get; set; }  
        // Price relation nav 
        public int? priceId { get; set; }
        public Price Price { get; set; }
        [JsonConverter(typeof(CustomTimeSpanConverter))]
        public TimeSpan OpenTime { get; set; }

        [JsonConverter(typeof(CustomTimeSpanConverter))]
        public TimeSpan CloseTime { get; set; }

        public string Duration { get; set; }
        public string? PictureUrl { get; set; }

        public int? locationId { get; set; }
        public Location? Location { get; set; }

        //nav prop category and fk
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<PlaceImage> Images { get; set; } = new List<PlaceImage>();

    }
}
