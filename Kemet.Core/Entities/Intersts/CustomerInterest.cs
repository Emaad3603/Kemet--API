using Kemet.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.Intersts
{
    public class CustomerInterest
    {
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
