using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Services.Interfaces;
using Kemet.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Services
{
    public class HomeServices : IHomeServices
    {
        private readonly AppDbContext _context;
        private readonly IGenericRepository<Place> _placesRepo;
        private readonly IGenericRepository<Activity> _activityRepo;
        private readonly IConfiguration _configuration;

        public HomeServices(
                           AppDbContext context
                          ,IGenericRepository<Place> placesRepo
                          ,IGenericRepository<Activity> ActivityRepo
                          ,IConfiguration configuration
                          )
        {
            _context = context;
            _placesRepo = placesRepo;
            _activityRepo = ActivityRepo;
            _configuration = configuration;
        }

        public async Task<List<Activity>> GetActivities()
        {
           var activites = await _activityRepo.GetAllAsync();
           var result = activites.Take(10).ToList();
           return result;
        }

        public async Task<List<Activity>> GetNearbyActivities(AppUser user)
        {
            var userLocation = user.Location; // Assuming user.Location is a Point

            // Initialize radius (10 km)
            double radius = 10000; // 10 km in meters
            List<Activity> nearbyActivities = new List<Activity>();

            // Fetch activities within the radius, increasing the radius until at least 5 activities are found
            while (nearbyActivities.Count < 10)
            {
                // Fetch activities within the current radius
                var activitiesWithinRadius = await _context.Activities
                    .Where(a => a.Location.Coordinates.Distance(userLocation) <= radius)
                    .ToListAsync();

                // Add activities to the nearbyActivities list
                nearbyActivities.AddRange(activitiesWithinRadius);

                // If no activities are found, break the loop to avoid infinite looping
                if (!activitiesWithinRadius.Any())
                {
                    break;
                }

                // Increase the radius by 10 km
                radius += 10000;
            }
            return nearbyActivities;

        }

        public async Task<List<Place>> GetNearbyPlaces(AppUser user)
        {
            var userLocation = user.Location; // Assuming user.Location is a Point

            // Initialize radius (10 km)
            double radius = 10000; // 10 km in meters
            List<Place> nearbyPlaces = new List<Place>();

            // Fetch places within the radius, increasing the radius until at least 5 places are found
            while (nearbyPlaces.Count < 10)
            {
                // Fetch places within the current radius
                var placesWithinRadius = await  _context.Places
                    .Where(p => p.Location.Coordinates.Distance(userLocation) <= radius)
                    .ToListAsync();

                // Add places to the nearbyPlaces list
                nearbyPlaces.AddRange(placesWithinRadius);

                // If no places are found, break the loop to avoid infinite looping
                if (!placesWithinRadius.Any())
                {
                    break;
                }

                // Increase the radius by 10 km
                radius += 10000;
            }
         
            return nearbyPlaces;
        }

        public async Task<List<Place>> GetPlaces()
        {
          var places =   await   _placesRepo.GetAllAsync();
          var result =  places.Take(10).ToList();
          return result;
        }

    }
}
