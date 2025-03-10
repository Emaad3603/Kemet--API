using Kemet.Core.Entities;
using Kemet.Core.Entities.WishlistEntites;
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
    public class WishlistRepository : GenericRepository<Wishlist>, IWishlistRepository
    {
        public WishlistRepository(AppDbContext context ) : base( context )
        {
            
        }

        public async Task AddActivityToWishlist(string userId, int activityId)
        {
            var wishlist =   await _context.Wishlists.Where(w => w.UserID == userId).FirstOrDefaultAsync();
            if ( wishlist == null )
            {
                wishlist = new Wishlist()
                {
                    UserID = userId,
                };
                await _context.Wishlists.AddAsync(wishlist);
                await _context.SaveChangesAsync();
            }
            var NeWshlist = await _context.Wishlists.Where(w => w.UserID == userId).FirstOrDefaultAsync();
            var activity = await _context.Activities.Where(A => A.Id == activityId).FirstOrDefaultAsync();
         
                var wishlistActivity = new WishlistActivites()
                {
                    WishlistID = NeWshlist.Id,
                    ActivityID = activityId,
                    Activity = activity
                };
                await _context.WishlistActivites.AddAsync(wishlistActivity);
                await _context.SaveChangesAsync();
            
           

        }
        public async Task<bool> RemoveActivityFromWishlist(string userId, int activityId)
        {
            var wishlist = await _context.Wishlists
                .Include(w => w.Activities)
                .FirstOrDefaultAsync(w => w.UserID == userId);

            if (wishlist == null) return false;

            var activity = wishlist.Activities.FirstOrDefault(a => a.Id == activityId);
            if (activity == null) return false;

            wishlist.Activities.Remove(activity);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task AddPlaceToWishlist(string userId, int placeId)
        {
            var wishlist = await _context.Wishlists.Where(w => w.UserID == userId).FirstOrDefaultAsync();
            if (wishlist == null)
            {
                wishlist = new Wishlist()
                {
                    UserID = userId,
                };
                await _context.Wishlists.AddAsync(wishlist);
                await _context.SaveChangesAsync();
            }
            var NeWshlist = await _context.Wishlists.Where(w => w.UserID == userId).FirstOrDefaultAsync();
            var place = await _context.Places.Where(p => p.Id == placeId).FirstOrDefaultAsync();
            var wishlistPlaces = new WishlistPlaces()
            {
                WishlistID = NeWshlist.Id,
                PlaceID = placeId ,
                Place = place
            };
            await _context.WishlistPlaces.AddAsync(wishlistPlaces);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> RemovePlaceFromWishlist(string userId, int PlaceId)
        {
            var wishlist = await _context.Wishlists
                .Include(w => w.Places)
                .FirstOrDefaultAsync(w => w.UserID == userId);

            if (wishlist == null) return false;

            var place = wishlist.Places.FirstOrDefault(a => a.Id == PlaceId);
            if (place == null) return false;

            wishlist.Places.Remove(place);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task AddPlanToWishlist(string userId, int planId)
        {
            var wishlist = await _context.Wishlists.Where(w => w.UserID == userId).FirstOrDefaultAsync();
            if (wishlist == null)
            {
                wishlist = new Wishlist()
                {
                    UserID = userId,
                };
                await _context.Wishlists.AddAsync(wishlist);
                await _context.SaveChangesAsync();
            }
            var NeWshlist = await _context.Wishlists.Where(w => w.UserID == userId).FirstOrDefaultAsync();
            var plans = await _context.TravelAgencyPlans.Where(TP => TP.Id == planId).FirstOrDefaultAsync();

            var wishlistPlans = new WishlistPlans()
            {
                WishlistID = NeWshlist.Id,
                TravelAgencyPlanID = planId,
                TravelAgencyPlan = plans
                
            };
            await _context.WishlistPlans.AddAsync(wishlistPlans);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> RemovePlanFromWishlist(string userId, int PlanId)
        {
            var wishlist = await _context.Wishlists
                .Include(w => w.Plans)
                .FirstOrDefaultAsync(w => w.UserID == userId);

            if (wishlist == null) return false;

            var Plan = wishlist.Plans.FirstOrDefault(a => a.Id == PlanId);
            if (Plan == null) return false;

            wishlist.Plans.Remove(Plan);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Wishlist> GetUserWishlist(string userID)
        {
            var wishlist = await _context.Wishlists.Where(w => w.UserID == userID).FirstOrDefaultAsync();
            if (wishlist == null)
            {
                wishlist = new Wishlist()
                {
                    UserID = userID,
                };
                await _context.Wishlists.AddAsync(wishlist);
                await _context.SaveChangesAsync();
                return wishlist;
            }
            var NeWshlist = await _context.Wishlists.Where(w => w.UserID == userID).
                Include(w=>w.Plans).ThenInclude(p=>p.TravelAgencyPlan).
                Include(w=>w.Places).ThenInclude(p=>p.Place).
                Include(w=>w.Activities).ThenInclude(a=>a.Activity)
                .FirstOrDefaultAsync();

            return NeWshlist;
        }
    }
}
