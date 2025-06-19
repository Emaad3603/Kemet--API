using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Services.Interfaces
{
    public interface IHomeServices
    {
        Task<List<Place>> GetNearbyPlaces(AppUser user);
        Task<List<Place>> GetPlaces();

        Task<List<Activity>> GetNearbyActivities(AppUser user);

        Task<List<Activity>> GetActivities();

        Task<List<Activity>> GetActivitesInCairo();

        Task<List<Activity>> GetActivityHiddenGems();

        Task<List<Activity>> GetTopRatedActivities();



        Task<List<Place>> GetPlacesInCairo();
        Task<List<Place>> GetTopRatedPlaces();
        Task<List<Place>> GetPlacesHiddenGems();

        Task<List<Place>> GetPlacesByCustomerInterests(string customerId);
        Task<List<Activity>> GetActivitiesByCustomerInterests(string customerId);
    }
}
