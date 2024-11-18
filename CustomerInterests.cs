using System;
namespace Kemet.Core.Entities
{
    public class CustomerIntersts
    {



        public string CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
