using FoodAPI.Models.HEREDto;

namespace FoodAPI.Interfaces
{
    public interface IMapRoutingService
    {
        Task<TravelSummary?> GetShortestDistance
            (float latOrigin, float lngOrigin, float latArrival, float lngArrival);
    }
}
