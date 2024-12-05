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
