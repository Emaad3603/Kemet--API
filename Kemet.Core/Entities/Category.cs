using Kemet.Core.Entities.Intersts;
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
        public string CategoryType { get; set; }
        //Navigations PRoperties
        public ICollection<Activity> Activity { get; set; }
        public ICollection<Place> Place { get; set; }
        public ICollection<CustomerInterest> CustomerInterests { get; set; } = new List<CustomerInterest>();
    }
}
