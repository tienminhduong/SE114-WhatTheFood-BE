using FoodAPI.Interfaces;
using HERE.Api;

namespace FoodAPI.Services;

public class MapRoutingService : IMapRoutingService
{
    private string accessToken;

    MapRoutingService()
    {
        var cred = HereCredentials.FromFile("credentials.properties");
        IHereTokenFactory hereTokenFactory = new HereTokenFactory();
        HereToken token = hereTokenFactory.CreateToken(cred);

        accessToken = token.AccessToken;
    }
}
