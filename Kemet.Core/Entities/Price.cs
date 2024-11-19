using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities
{
    public class Price : BaseEntity
    {
        public decimal? EgyptianAdult { get; set; }

        public decimal? EgyptianStudent { get; set; }

        public decimal? TouristAdult { get; set; }

        public decimal? TouristStudent { get;  set; }
    }
}
