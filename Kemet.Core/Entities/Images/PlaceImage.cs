using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.Images
{
    public class PlaceImage : BaseEntity
    {

        public string ImageUrl { get; set; }
        public int PlaceId { get; set; }
        public Place Place { get; set; }  // Navigation property
    }
}
