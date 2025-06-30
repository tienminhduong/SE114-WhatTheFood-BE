using FoodAPI.Interfaces;
using FoodAPI.Models;
using FoodAPI.Models.HEREDto;
using FoodAPI.Repositories;
using HERE.Api;
using System.Text.Json;

namespace FoodAPI.Services;

public class MapRoutingService(
    IConfiguration config,
    IRestaurantRepository restaurantRepository,
    IFoodItemRepository foodItemRepository
    ) : IMapRoutingService
{
    private string accessToken =
        (new HereTokenFactory())
        .CreateToken(HereCredentials.FromFile("credentials.properties")).AccessToken;

    public async Task<IEnumerable<FoodRecommendDto>> GetRecommendFoodByLocation(
        double latitude, double longitude)
    {
        double nearbyLimitDistance = 5; //5 km

        var restaurantList = await restaurantRepository.GetRestaurantsAsync();
        Dictionary<int, TravelSummary> nearbyRestaurant = [];


        foreach (var r in restaurantList)
        {
            var summary = await GetShortestDistance(
                latitude,
                longitude,
                r.Address!.Latitude,
                r.Address!.Longitude);

            if (summary == null)
                continue;

            if (summary.length / 1000f <= nearbyLimitDistance)
                nearbyRestaurant.Add(r.Id, summary);
        }


        List<FoodRecommendDto> result = [];
        foreach (var r in nearbyRestaurant)
        {
            var restaurant = (await restaurantRepository.GetRestaurantByIdAsync(r.Key,
                includeFoodItems: true))!;
            foreach (var fi in restaurant.FoodItems)
            {
                var item = new FoodRecommendDto
                {
                    FoodId = fi.Id,
                    ImgUrl = fi.CldnrUrl,
                    Name = fi.FoodName,
                    RestaurantName = restaurant.Name,
                    DistanceInKm = r.Value.length / 1000f,
                    DistanceInTime = r.Value.duration / 60,
                };

                item.Rating = (await foodItemRepository.GetFoodItemAvgRating(fi.Id));

                result.Add(item);
            }
        }

        var sortedResult = result.OrderBy(r => r.DistanceInKm);
        return sortedResult;
    }

    public async Task<TravelSummary?> GetShortestDistance(
        double latOrigin, double lngOrigin, double latArrival, double lngArrival)
    {
        string? url = config["HEREMap:routingBaseUrl"]
            ?? throw new Exception("No routing url found");

        string @params = $"transportMode=car" +
            $"&origin={latOrigin},{lngOrigin}" +
            $"&destination={latArrival},{lngArrival}" +
            "&return=travelSummary";
        try
        {
            var result =
                await HttpService.GetAsync<RouteFindingResultDto>(accessToken, $"{url}?{@params}");

            return result?.routes?.FirstOrDefault()?.sections?.FirstOrDefault()?.travelSummary;
        }
        catch
        {
            return null;
        }
    }
}
