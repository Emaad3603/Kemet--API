using Kemet.Core.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Services.Interfaces
{
    public interface ITravelAgencyService
    {
        Task<ICollection<TravelAgencyPlan>> GetTravelAgencyPlans(string travelAgencyID , IConfiguration configuration);

        Task<ICollection<Review>> GetTravelAgencyReviews(string travelAgencyID , IConfiguration configuration);
    }
}
