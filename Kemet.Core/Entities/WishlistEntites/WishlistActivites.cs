using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.WishlistEntites
{
    public class WishlistActivites : BaseEntity
    {
        public int WishlistID { get; set; }

        

        public int ActivityID { get; set; }

        public Activity Activity { get; set; }


    }
}
