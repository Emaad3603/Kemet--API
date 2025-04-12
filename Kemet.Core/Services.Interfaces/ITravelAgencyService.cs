using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Entities.ModelView;
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

        Task<ICollection<TravelAgencyBookedCustomersModelView>> GetCustomersAsync(string TravelAgencyName);

        Task<(double satisfactionRate, Dictionary<int, int> ratingCounts)> CalculateSatisfactionRateAsync(string TravelAgencyId);

        Task<Dictionary<string, decimal>> GetMonthlyRevenueAsync(string agencyName);

        Task<Dictionary<string, int>> GetTopBookedPlansAsync(string agencyName);

        Task<(bool IsSuccess, string Message, TravelAgencyPlan? Plan)> AddPlanAsync(CreatePlanDto dto);

        Task<(bool IsSuccess, string Message)> UpdatePlanAsync(UpdatePlanDto dto);
    }
}
