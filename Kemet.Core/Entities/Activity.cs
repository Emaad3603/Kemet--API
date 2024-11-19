using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities
{
    public class Activity :BaseEntity
    {
        public string Name { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public int GroupSize { get; set; }
        public string PictureUrl { get; set; }

        //has one location
        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        //one place has many activities
        public int? PlaceId { get; set; }
        public Place? Place { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
