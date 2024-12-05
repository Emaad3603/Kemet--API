using Kemet.Core.Entities;
using Kemet.Core.Repositories.InterFaces;
using Kemet.Core.Specifications;
using Kemet.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
      
        public ReviewRepository(AppDbContext context):base(context) 
       
        {
            
        }

        public async Task AddReviewAsync (Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            if (review.TravelAgencyPlanId != null)
            {
                var averageRate = await _context.Set<Review>()
                      .Where(r => r.TravelAgencyPlanId == review.TravelAgencyPlanId)
                      .AverageAsync(r => (double?)r.Rating) ?? 0.0;
                var plan =  await  _context.TravelAgencyPlans.Where(t => t.Id == review.TravelAgencyPlanId).FirstOrDefaultAsync();
                plan.AverageRating = averageRate;
                plan.RatingsCount = plan.RatingsCount++; 
                _context.TravelAgencyPlans.Update(plan);
                await _context.SaveChangesAsync();

            }
            if(review.PlaceId != null)
            {
                var averageRate = await _context.Set<Review>()
                     .Where(r => r.PlaceId == review.PlaceId)
                     .AverageAsync(r => (double?)r.Rating) ?? 0.0;
                var place = await _context.Places.Where(t => t.Id == review.PlaceId).FirstOrDefaultAsync();
                place.AverageRating = averageRate;
                place.RatingsCount = place.RatingsCount++;
                _context.Places.Update(place);
                await _context.SaveChangesAsync();
            }
            if(review.ActivityId != null)
            {
                var averageRate = await _context.Set<Review>()
                    .Where(r => r.ActivityId == review.ActivityId)
                    .AverageAsync(r => (double?)r.Rating) ?? 0.0;
                var activity = await _context.Activities.Where(t => t.Id == review.ActivityId).FirstOrDefaultAsync();
                activity.AverageRating = averageRate;
                activity.RatingsCount = activity.RatingsCount++;
                _context.Activities.Update(activity);
                await _context.SaveChangesAsync();
            }
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
