using Kemet.Core.Entities.Identity;
using Kemet.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kemet.Core.Entities.ModelView;
using Kemet.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Kemet.Core.Entities;

namespace Kemet.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly string _pPuploadsFolder;
        private readonly string _bGuploadsFolder;
        private readonly AppDbContext _context;

        public ProfileService(UserManager<AppUser> userManager, string PPuploadsFolder , string BGuploadsFolder , AppDbContext context)
        {
            _userManager = userManager;
            _pPuploadsFolder = PPuploadsFolder;
            _bGuploadsFolder = BGuploadsFolder;
            _context = context;
        }

        public async Task<AdventureDTO> GetAdventureModeSuggest(AppUser? user)
        {
            try
            {
                var currentTime = DateTime.UtcNow.TimeOfDay;
                var random = new Random();
                var allPlaces = await _context.Places.Include(p => p.Price).Include(p=>p.Images).ToListAsync();
                var nearbyPlaces = new  List<Place>();
              
                if (user is null || user.Location is null)
                {
                    nearbyPlaces = await _context.Places.ToListAsync();

                }
                else
                {
                    var userLocation = user.Location; // Assuming user.Location is a Point

                    // Initialize radius (10 km)
                    double radius = 10000; // 10 km in meters


                    // Fetch places within the radius, increasing the radius until at least 5 places are found
                    while (nearbyPlaces.Count < 10)
                    {
                        // Fetch places within the current radius
                        var placesWithinRadius = await _context.Places
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
                }
                var openPlaces = GetOpenPlaces(nearbyPlaces);
                var ranNumber = random.Next(1,openPlaces.Count() + 1);
                /*var adventurePlace = openPlaces.Where(p => p.Id == ranNumber).FirstOrDefault();*/
                var p = openPlaces.ToArray();
                var adventurePlace = p[ranNumber];

                if (adventurePlace != null)
                {
                    var actvityLen = await _context.Activities.Where(a => a.PlaceId == adventurePlace.Id).CountAsync() + 1;
                    var actvities = await _context.Activities.Where(a => a.PlaceId == adventurePlace.Id).Include(a => a.Price).Include(a=>a.Images).ToListAsync();
                    ranNumber = random.Next(1,actvityLen);
                    var A = actvities.ToArray();
                    //var adventureActivity = actvities.Where(a => a.Id == ranNumber).FirstOrDefault();
                    var adventureActivity = A[ranNumber];
                    if (adventureActivity != null)
                    {
                        return new AdventureDTO()
                        {
                            Place = adventurePlace,
                            Activity = adventureActivity,
                        };
                    }
                    return new AdventureDTO()
                    {
                        Place = adventurePlace,
                    };
                }
                return new AdventureDTO();
            }catch(Exception e)
            {
                return new AdventureDTO();
               
            }

        }
        public IEnumerable<Place> GetOpenPlaces(IEnumerable<Place> places)
        {
            var currentTime = DateTime.UtcNow.TimeOfDay; // Current time as TimeSpan
            return places.Where(place =>
            {
                // Handle scenarios where CloseTime is past midnight (e.g., open overnight)
                if (place.CloseTime < place.OpenTime)
                {
                    return currentTime >= place.OpenTime || currentTime < place.CloseTime;
                }
                return currentTime >= place.OpenTime && currentTime < place.CloseTime;
            }).ToList();
        }

        public async Task<(bool IsSuccess, string Message, string ImageUrl)> UploadProfileImageAsync(string userEmail, IFormFile? profileImage, IFormFile? backgroundImage)
        {

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return (false, "User not found.", null);
            }

            // Ensure the directory exists
            if (!Directory.Exists(_pPuploadsFolder))
            {
                Directory.CreateDirectory(_pPuploadsFolder);
            }
            if (!Directory.Exists(_bGuploadsFolder))
            {
                Directory.CreateDirectory(_bGuploadsFolder);
            }

            if (profileImage != null)
            {
                string pPuniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(profileImage.FileName);
                string pPfilePath = Path.Combine(_pPuploadsFolder, pPuniqueFileName);
                using (var fileStream = new FileStream(pPfilePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(fileStream);
                }
                user.ImageURL = "/uploads/ProfileImages/" + pPuniqueFileName;

            }
            if(backgroundImage != null)
            {
                string bGuniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(backgroundImage.FileName);
                string bGfilePath = Path.Combine(_bGuploadsFolder, bGuniqueFileName);

                using (var fileStream = new FileStream(bGfilePath, FileMode.Create))
                {
                    await backgroundImage.CopyToAsync(fileStream);
                }

                user.BackgroundImageURL = "/uploads/BackGroundImages/" + bGuniqueFileName;
            }
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return (false, "An error occurred while updating the user profile.", null);
            }

            return (true, "Profile image uploaded successfully.", user.ImageURL);
        }
    }
}
