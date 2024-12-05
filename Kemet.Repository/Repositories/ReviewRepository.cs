using Kemet.Core.Entities;
using Kemet.Core.Repositories.InterFaces;
using Kemet.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context):base(context) 
       
        {
            _context = context;
        }
        public async Task<double> GetAverageRatingForActivityAsync(int activityId)
        {
            return await _context.Set<Review>()
            .Where(r => r.ActivityId == activityId)
            .AverageAsync(r => (double?)r.Rating) ?? 0.0;
        }

        public async Task<double> GetAverageRatingForPlaceAsync(int placeId)
        {
            return await _context.Set<Review>()
            .Where(r => r.PlaceId == placeId)
            .AverageAsync(r => (double?)r.Rating) ?? 0.0;
        }

        public async Task<double> GetAverageRatingForTravelAgencyPlanAsync(int planId)
        {
            return await _context.Set<Review>()
             .Where(r => r.TravelAgencyPlanId == planId)
             .AverageAsync(r => (double?)r.Rating) ?? 0.0;
        }

        public async Task<IReadOnlyList<Review>> GetReviewsForActivityAsync(int activityId)
        {
            return await _context.Set<Review>()
           .Where(r => r.ActivityId == activityId)
           .ToListAsync();
        }

        public async Task<IReadOnlyList<Review>> GetReviewsForPlaceAsync(int placeId)
        {
            return await _context.Set<Review>()
             .Where(r => r.PlaceId == placeId)
             .ToListAsync();
        }

        public async Task<IReadOnlyList<Review>> GetReviewsForTravelAgencyPlanAsync(int planId)
        {
            return await _context.Set<Review>()
             .Where(r => r.TravelAgencyPlanId == planId)
             .ToListAsync();
        }
    }
}
