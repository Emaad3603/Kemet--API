using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kemet.Core.Entities
{
    public class Location :BaseEntity
    {
        public string Address { get; set; }

        [JsonPropertyName("LocationLink")] // Map "LocationLink" JSON key
        public string? LocationLink { get; set; }

        //[JsonPropertyName("PlaceLatitude")] // Map "PlaceLatitude" JSON key
        //public string? PlaceLatitude { get; set; }

        //[JsonPropertyName("PlaceLongitude")] // Map "PlaceLongitude" JSON key
        //public string? PlaceLongitude { get; set; }
        public Point Coordinates { get; set; } // Spatial column

    }
}
