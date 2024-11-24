using Kemet.Core.Entities;
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

            if (!context.Activities.Any())
            {
                var activityData = File.ReadAllText("../Kemet.Repository/Data/DataSeed/SeedingData/Activities.json");

                // Deserialize JSON into ActivityDto
                var activityDtos = JsonSerializer.Deserialize<List<ActivitySeedDto>>(activityData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true  // Allow case-insensitive property names
                });

                if (activityDtos != null)
                {
                    // Map DTOs to Activity entities
                    var activities = activityDtos.Select(dto => new Activity
                    {
                        Id = dto.Id,
                        Name = dto.Name,
                        Description = dto.Description,
                       
                        Duration = dto.Duration,
                        OpenTime = dto.OpenTime,
                        CloseTime = dto.CloseTime,
                        priceId = dto.PriceId,
                        LocationId = dto.LocationId,
                        CategoryId = dto.CategoryId
                    }).ToList();

                    await context.Activities.AddRangeAsync(activities);
                }
            }

            // Seed Places
            if (!context.Places.Any())
            {
                var placeData = File.ReadAllText("../Kemet.Repository/Data/DataSeed/SeedingData/Places.json");

                // Deserialize JSON into PlaceDto
                var placeDtos = JsonSerializer.Deserialize<List<PlaceSeedDto>>(placeData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true  // Allow case-insensitive property names
                });

                if (placeDtos != null)
                {
                    // Map DTOs to Place entities
                    var places = placeDtos.Select(dto => new Place
                    {
                        Id = dto.Id,
                        Name = dto.Name,
                        CultureTips = dto.CulturalTip,
                        Description = dto.Description,
                        Duration = dto.Duration,
                        OpenTime = dto.OpenTime,
                        CloseTime = dto.CloseTime,
                        
                        priceId = dto.PriceId,
                        locationId = dto.LocationId,
                        CategoryId = dto.CategoryId
                    }).ToList();

                    await context.Places.AddRangeAsync(places);
                }
            }

            // Save changes to the database
            await context.SaveChangesAsync();
          
        }
    }
}
