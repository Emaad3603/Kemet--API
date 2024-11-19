using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public int Duration { get; set; }
        public string PictureUrl { get; set; }

        public int? locationId { get; set; }
        public Location? Location { get; set; }

        //nav prop category and fk
        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
