using Azure.Core;
using System.Runtime.InteropServices;

namespace FoodAPI.Services;

public static class HttpService
{
    public static async Task<T?> PostAsync<T>(string accessToken, string url, string body)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(body)
        };

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public static async Task<T?> GetAsync<T>(string accessToken, string url)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>();
    }
}
