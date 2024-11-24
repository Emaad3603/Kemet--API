using Kemet.APIs.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.DataSeed.DataSeedingDTOs
{
    public class PlaceSeedDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CulturalTip { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }

        [JsonConverter(typeof(CustomTimeSpanConverter))]  // Handle mixed time formats
        public TimeSpan OpenTime { get; set; }

        [JsonConverter(typeof(CustomTimeSpanConverter))]  // Handle mixed time formats
        public TimeSpan CloseTime { get; set; }

       
        public int PriceId { get; set; }
        public int LocationId { get; set; }
        public int CategoryId { get; set; }  // Only the CategoryId is included
    }
}
