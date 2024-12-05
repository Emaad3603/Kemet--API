using Kemet.Core.Entities.WishlistEntites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Repositories.InterFaces
{
    public interface IWishlistRepository
    {
        public Task<Wishlist> GetUserWishlist(string userID);
        public Task AddPlaceToWishlist(string userId , int placeId);

        public Task AddActivityToWishlist (string userId , int activityId);

        public Task AddPlanToWishlist (string userId , int planId);
    }
}
