using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Repositories.InterFaces;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Services.Interfaces;
using Kemet.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
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

        public async Task<List<Activity>> GetActivitesInCairo()
        {
            // Define Cairo's location (Longitude, Latitude)
            var cairoLocation = new Point(31.2357, 30.0444) { SRID = 4326 };

            // Start with a reasonable radius (20 km)
            double radius = 20000; // 20 km in meters
            double maxRadius = 100000; // 100 km max radius
            List<Activity> CairoActivities = new List<Activity>();

            while (CairoActivities.Count < 25 && radius <= maxRadius)
            {
                var activitiesWithinRadius = await _context.Activities.AsNoTracking()
                    .Where(a => a.Location.Coordinates.Distance(cairoLocation) <= radius)
                    .OrderBy(a => a.Location.Coordinates.Distance(cairoLocation)) // Sort by nearest first
                    .ToListAsync()
                    ;

                CairoActivities.AddRange(activitiesWithinRadius);

                // Stop increasing radius if enough activities are found
                if (CairoActivities.Count >= 25)
                {
                    break;
                }

                // Increase the radius gradually (e.g., by 10 km)
                radius += 10000;
            }


            return CairoActivities;        }

        public async Task<List<Activity>> GetActivities()
        {
           var activites = await _activityRepo.GetAllAsync();
           var result = activites.ToList();
           return result;
        }

        public async Task<List<Activity>> GetActivityHiddenGems()
        {
            var activities = await _context.Activities.AsNoTracking().Where(a=>a.CategoryId == 20).ToListAsync();
            return activities;
        }
        public async Task<List<Activity>> GetTopRatedActivities()
        {
            var activities = await _context.Activities.AsNoTracking().OrderByDescending(a=>a.AverageRating).ToListAsync();
            var result = activities.Take(20).ToList();
            return result;
        }
        public async Task<List<Activity>> GetNearbyActivities(AppUser user)
        {
            var userLocation = user.Location; // Assuming user.Location is a Point

            // Initialize radius (10 km)
            double radius = 10000; // 10 km in meters
            List<Activity> nearbyActivities = new List<Activity>();

            // Fetch activities within the radius, increasing the radius until at least 5 activities are found
            while (nearbyActivities.Count < 20)
            {
                // Fetch activities within the current radius
                var activitiesWithinRadius = await _context.Activities.AsNoTracking()
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
          var result =  places.ToList();
          return result;
        }

        public async Task<List<Place>> GetPlacesHiddenGems() 
        {
            var places = await _context.Places.Where(p => p.CategoryId == 19).ToListAsync();
            return places;
        }
        public async Task<List<Place>> GetTopRatedPlaces()
        {
            var places = await _context.Places.OrderByDescending(p => p.AverageRating).ToListAsync();
            var result= places.Take(20).ToList();
            return result;
        }
        public async Task<List<Place>> GetPlacesInCairo() 
        {
            //Define Cairo's Location (Longitude ,Latitude)
            var CairoLocation = new Point(31.2357, 30.0444) { SRID = 4326 };

            //start with a reasonable radius (20 km)
            double radius = 20000;
            double maxRadius = 100000;
            List<Place> CairoPlaces = new List<Place>();

            while (CairoPlaces.Count < 25 && radius <= maxRadius)
            {
                var placesWithinRadius = await _context.Places
                        .Where(P => P.Location.Coordinates.Distance(CairoLocation) <= radius)
                        .OrderBy(p => p.Location.Coordinates.Distance(CairoLocation))
                        .ToListAsync();
                CairoPlaces.AddRange(placesWithinRadius);
                if (CairoPlaces.Count >= 25)
                {
                    break;
                }
                //Increase the radius gradually(e.g. ,by 10km)
                radius += 10000;
            }
            return CairoPlaces;
            }

        public async Task<List<Place>> GetPlacesByCustomerInterests(string customerId)
        {
            // Get category IDs from the customer interests form
            var categoryIds = await _context.CustomerInterests
                .Where(ci => ci.CustomerId == customerId)
                .Select(ci => ci.CategoryId)
                .ToListAsync();

            // Get places that match any of those category IDs
            var places = await _context.Places
                .Where(p => categoryIds.Contains(p.CategoryId))
                .ToListAsync();

            return places;
        }

        public async Task<List<Activity>> GetActivitiesByCustomerInterests(string customerId)
        {
            // Get category IDs from the customer interests form
            var categoryIds = await _context.CustomerInterests
                .Where(ci => ci.CustomerId == customerId)
                .Select(ci => ci.CategoryId)
                .ToListAsync();

            // Get places that match any of those category IDs
            var Activities = await _context.Activities
                .Where(p => categoryIds.Contains(p.CategoryId))
                .ToListAsync();

            return Activities;
        }

      
    }
    }

