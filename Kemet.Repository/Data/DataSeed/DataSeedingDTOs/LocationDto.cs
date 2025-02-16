using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.DataSeed.DataSeedingDTOs
{
    public class LocationDto
    {
        public int Id { get; set; } = 1;
        public string? Address { get; set; }
        public string? LocationLink { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

}
