using Kemet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kemet.Repository.Data
{
    public static class HomeDbContextSeed
    { //seed data
        
        public static async Task SeedAsync(AppDbContext _context) 
        {  // travel agency plan data
           if (_context.TravelAgencyPlans.Count() == 0)
            {
                // 1.Read Data From Json File
                var TravelAgencyPlanData = File.ReadAllText("../Kemet.Repository/Data/DataSeed/TravelAgencyPlan/TravelAgencyPlans.json");
                //2.Convert Json String To the Needed Type
                var pLans = JsonSerializer.Deserialize<List<TravelAgencyPlan>>(TravelAgencyPlanData);
                if (pLans?.Count > 0)
                {
                    foreach (var plan in pLans)
                    {
                        _context.Set<TravelAgencyPlan>().Add(plan);
                    }
                    await _context.SaveChangesAsync();
                }
           }
            //=======================================================


        }
    }
}
