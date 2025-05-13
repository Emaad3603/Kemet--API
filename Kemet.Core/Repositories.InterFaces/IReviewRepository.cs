using Kemet.Core.Entities;
using Kemet.Core.RepositoriesInterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Repositories.InterFaces
{
    public interface IReviewRepository : IGenericRepository<Review> 
    {
        Task AddReviewAsync(Review review);
        Task<double> GetAverageRatingForPlaceAsync(int placeId);
        Task<double> GetAverageRatingForActivityAsync(int activityId);
        Task<double> GetAverageRatingForTravelAgencyPlanAsync(int planId);
        Task<IReadOnlyList<Review>> GetReviewsForPlaceAsync(int placeId);
        Task<IReadOnlyList<Review>> GetReviewsForActivityAsync(int activityId);

        Task<IReadOnlyList<Review>> GetReviewsForTravelAgencyPlanAsync(int planId);

        Task<IEnumerable<Review>> GetAllReviewsForAdminAsync();
    }
}
