using Kemet.Core.Entities;

namespace Kemet.Core.Services.Interfaces
{
    public interface IPlacesServices
    {
        Task<List<Place>> GetAllPlacesAsync();
        Task<Place> GetPlaceByIdAsync(int id);
        Task<List<Place>> GetPlacesByCategoryAsync(string category);
        Task<List<Place>> GetPlacesByLocationAsync(string location);
        Task<List<Place>> GetFeaturedPlacesAsync();
        Task<List<Place>> GetPopularPlacesAsync();
        Task<bool> IncrementVisitCountAsync(int id);
    }
}