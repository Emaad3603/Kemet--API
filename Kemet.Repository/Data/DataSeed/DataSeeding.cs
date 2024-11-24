using Kemet.Core.Entities;
using Kemet.Core.Entities.Images;
using Kemet.Repository.Data.DataSeed.DataSeedingDTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.DataSeed
{
    public static class DataSeeding
    {
       
        public static async Task SeedDataAsync(AppDbContext context)
        {
            // Ensure the database is created
            await context.Database.EnsureCreatedAsync();
       
            // Seed Locations
            if (!context.Locations.Any())
            {
                var locationData = File.ReadAllText("../Kemet.Repository/Data/DataSeed/SeedingData/Locations.json");
                locationData = JsonCleaner.CleanJson(locationData); // Clean JSON using JsonCleaner
                var locations = JsonSerializer.Deserialize<List<Location>>(locationData);
                if (locations != null)
                {
                    await context.Locations.AddRangeAsync(locations);
                }
            }

            await context.SaveChangesAsync();

            // Seed Prices
            if (!context.Prices.Any())
            {
                var priceData = File.ReadAllText("../Kemet.Repository/Data/DataSeed/SeedingData/Prices.json");
                priceData = JsonCleaner.CleanJson(priceData); // Clean JSON using JsonCleaner
                var prices = JsonSerializer.Deserialize<List<Price>>(priceData);
                if (prices != null)
                {
                    await context.Prices.AddRangeAsync(prices);
                }
            }

            await context.SaveChangesAsync();

            // Base directory for images
            var baseImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");

            if (!context.Places.Any())
            {
                var placeData = File.ReadAllText("../Kemet.Repository/Data/DataSeed/SeedingData/Places.json");
                var placesDto = JsonSerializer.Deserialize<List<PlaceSeedDto>>(placeData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (placesDto != null)
                {
                    foreach (var placeDto in placesDto)
                    {
                        // Map DTO to Entity
                        var place = new Place
                        {
                            Id = placeDto.Id,
                            Name = placeDto.Name,
                            CultureTips = placeDto.CulturalTip,
                            Description = placeDto.Description,
                            Duration = placeDto.Duration,
                            OpenTime = placeDto.OpenTime,
                            CloseTime = placeDto.CloseTime,
                            priceId = placeDto.PriceId,
                            locationId = placeDto.LocationId,
                            CategoryId = placeDto.CategoryId,
                        };

                        // Folder path for Place images
                        var folderPath = Path.Combine(baseImagePath, "Places", place.Name);
                        if (Directory.Exists(folderPath))
                        {
                            var imageFiles = Directory.GetFiles(folderPath);
                            var placeImages = imageFiles.Select(imageFile => new PlaceImage
                            {
                                ImageUrl = $"/Images/Places/{place.Name}/{Path.GetFileName(imageFile)}",
                                PlaceId = place.Id
                            }).ToList();

                            place.Images = placeImages;
                        }

                        await context.Places.AddAsync(place);
                    }
                }
               
            }

            await context.SaveChangesAsync();

            // Seed Activities
            if (!context.Activities.Any())
            {
                var activityData = File.ReadAllText("../Kemet.Repository/Data/DataSeed/SeedingData/Activities.json");
                var activitiesDto = JsonSerializer.Deserialize<List<ActivitySeedDto>>(activityData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (activitiesDto != null)
                {
                    foreach (var activityDto in activitiesDto)
                    {
                        // Map DTO to Entity
                        var activity = new Activity
                        {
                            Id = activityDto.Id,
                            Name = activityDto.Name,
                            CulturalTips = activityDto.CulturalTip ?? "No cultural tips available",
                            Description = activityDto.Description,
                            Duration = activityDto.Duration,
                            OpenTime = activityDto.OpenTime,
                            CloseTime = activityDto.CloseTime,
                            priceId = activityDto.PriceId,
                            LocationId = activityDto.LocationId,
                            CategoryId = activityDto.CategoryId,
                          //  PictureUrl = activityDto.PictureUrl // Optional main image URL
                        };

                        // Folder path for Activity images
                        var folderPath = Path.Combine(baseImagePath, "Activities", activity.Name);
                        if (Directory.Exists(folderPath))
                        {
                            var imageFiles = Directory.GetFiles(folderPath);
                            var activityImages = imageFiles.Select(imageFile => new ActivityImage
                            {
                                ImageUrl = $"/Images/Activities/{activity.Name}/{Path.GetFileName(imageFile)}",
                                ActivityId = activity.Id
                            }).ToList();

                            activity.Images = activityImages;
                        }

                        await context.Activities.AddAsync(activity);
                    }
                }
            }

            // Save changes to the database
            await context.SaveChangesAsync();
          
        }
    }
}
