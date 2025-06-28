using FoodAPI.Models.HEREDto;

namespace FoodAPI.Interfaces
{
    public interface IMapRoutingService
    {
        TravelSummaryDto GetShortestDistance(float lat1, float lon1, float lat2, float lon2);
    }
}
