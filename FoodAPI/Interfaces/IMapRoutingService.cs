using FoodAPI.Models;
using FoodAPI.Models.HEREDto;

namespace FoodAPI.Interfaces
{
    public interface IMapRoutingService
    {
        Task<TravelSummary?> GetShortestDistance
            (double latOrigin, double lngOrigin, double latArrival, double lngArrival);

        Task<IEnumerable<FoodRecommendDto>> GetRecommendFoodByLocation(
            double latitude, double longitude);
    }
}
