using FoodAPI.Interfaces;
using FoodAPI.Models.HEREDto;
using HERE.Api;
using System.Text.Json;

namespace FoodAPI.Services;

public class MapRoutingService(IConfiguration config) : IMapRoutingService
{
    private string accessToken =
        (new HereTokenFactory())
        .CreateToken(HereCredentials.FromFile("credentials.properties")).AccessToken;

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
