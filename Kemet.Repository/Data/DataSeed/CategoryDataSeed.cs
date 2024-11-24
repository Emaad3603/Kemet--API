using Kemet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Data.DataSeed
{
    public static class CategoryDataSeed
    {
        public static async Task SeedCategoriesAsync(AppDbContext context)
        {
            if (!context.Categories.Any())
            {
                // Seed Place Categories
                var placeCategories = new[]
                {
                new Category { CategoryName = "Historical", CategoryType = "Place" },
                new Category { CategoryName = "Resorts and Beaches", CategoryType = "Place" },
                new Category { CategoryName = "Nature Spots", CategoryType = "Place" },
                new Category { CategoryName = "Museums", CategoryType = "Place" },
                new Category { CategoryName = "Religious", CategoryType = "Place" },
                new Category { CategoryName = "Nile River Destinations", CategoryType = "Place" },
                new Category { CategoryName = "Desert Landscape", CategoryType = "Place" },
                new Category { CategoryName = "Entertainment", CategoryType = "Place" }
            };

                // Seed Activity Categories
                var activityCategories = new[]
                {
                new Category { CategoryName = "Diving Snorkeling", CategoryType = "Activity" },
                new Category { CategoryName = "Hiking", CategoryType = "Activity" },
                new Category { CategoryName = "Water Sports and Nile Activities", CategoryType = "Activity" },
                new Category { CategoryName = "Cultural Experience", CategoryType = "Activity" },
                new Category { CategoryName = "Adventure Activity", CategoryType = "Activity" },
                new Category { CategoryName = "Relaxation and Wellness", CategoryType = "Activity" },
                new Category { CategoryName = "Entertainment", CategoryType = "Activity" },
                new Category { CategoryName = "Safari", CategoryType = "Activity" },
                new Category { CategoryName = "Fancy Cafe", CategoryType = "Activity" },
                new Category { CategoryName = "Fancy Restaurant", CategoryType = "Activity" }
            };

                await  context.Categories.AddRangeAsync(placeCategories);
                await  context.Categories.AddRangeAsync(activityCategories);
                await context.SaveChangesAsync();
            }
        }
    }
}
