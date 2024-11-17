using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities
{
    public class Category:BaseEntity
    {
        public string CategoryName { get; set; }

        public List<Activity> Activity { get; set; }
        public List<Place> Place { get; set; }
    }
}
