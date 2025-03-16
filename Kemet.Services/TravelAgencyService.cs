using Kemet.Core.Entities;
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
    public class TravelAgencyService : ITravelAgencyService
    {
        private readonly AppDbContext _context;

        public TravelAgencyService(AppDbContext context)
        {
            _context = context;
        }
        public async  Task<ICollection<TravelAgencyPlan>> GetTravelAgencyPlans(string travelAgencyID , IConfiguration configuration)
        {
           var plans =  await _context.TravelAgencyPlans.Where(T => T.TravelAgencyId == travelAgencyID).Include(t=>t.Reviews).ToListAsync();
           foreach (var plan in plans)
            {
                plan.TravelAgency = null;
                plan.PictureUrl = $"{configuration["BaseUrl"]}{plan.PictureUrl}";
                foreach (var review in plan.Reviews)
                {
                    review.ImageUrl = $"{configuration["BaseUrl"]}{review.ImageUrl}";
                }
            }
            return plans;
        }

        public async Task<ICollection<Review>> GetTravelAgencyReviews(string travelAgencyID, IConfiguration configuration)
        {
            var reviews = await _context.Reviews.Where(T => T.TravelAgencyID == travelAgencyID).ToListAsync();
            foreach (var review in reviews)
            {   
                review.ImageUrl = $"{configuration["BaseUrl"]}{review.ImageUrl}";
            }
            return reviews;
           
        }

       
    }
}
