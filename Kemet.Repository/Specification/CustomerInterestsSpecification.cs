using Kemet.Core.Entities.Intersts;
using Kemet.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Specification
{
    public class CustomerInterestsSpecification : BaseSpecifications<CustomerInterest>
    {
        // Specification for getting interests by CustomerId
        public CustomerInterestsSpecification(string customerId)
            : base(ci => ci.CustomerId == customerId)
        {
            Includes.Add(ci => ci.Category);  // Eager load Category data
        }

        // Specification for getting interests by CustomerId and CategoryId
        public CustomerInterestsSpecification(string customerId, int categoryId)
            : base(ci => ci.CustomerId == customerId && ci.CategoryId == categoryId)
        {
            Includes.Add(ci => ci.Category);  // Eager load Category data
        }
    }
}
