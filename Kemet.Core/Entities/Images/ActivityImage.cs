using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.Images
{
    public class ActivityImage : BaseEntity
    {
      
        public string ImageUrl { get; set; }
        public int ActivityId { get; set; }
        public Activity Activity { get; set; }  // Navigation property
    }
}
