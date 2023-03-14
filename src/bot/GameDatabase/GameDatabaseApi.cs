using System.Net;
using System.Text.Json;
using bot.GameDatabase.Auth;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Polly;

namespace bot.GameDatabase;

public class GameDatabaseApi : IGameDatabaseApi
{
    private readonly IConfiguration _configuration;
    private readonly ITwitchAccessTokenService _twitchAccessTokenService;
    private readonly IMemoryCache _cache;
    private Dictionary<string, Guid> _cacheKeys = new();

    public GameDatabaseApi(ITwitchAccessTokenService twitchAccessTokenService, IMemoryCache cache)
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
        _twitchAccessTokenService = twitchAccessTokenService;
        _cache = cache;
    }

    public async Task<T> GenericFetch<T>(string url, string body)
    {
        var result = await FetchApi<T>(url, body);
        return result;
    }

    private async Task<T> FetchApi<T>(Url url, string body)
    {
        if (_cacheKeys.TryGetValue(body, out Guid cacheKey))
        {
            if (_cache.TryGetValue(cacheKey, out T cacheValue))
            {
                return cacheValue;
            }
        }

        var settings = _configuration.GetRequiredSection("Twitch").Get<TwitchOptions>();
        var token = await _twitchAccessTokenService.GetTwitchAccessTokenAsync(false);
        var clientId = settings.ClientId;

        var authPolicy = Policy
            .Handle<FlurlHttpException>(r => r.StatusCode is (int)HttpStatusCode.Unauthorized)
            .RetryAsync(
                1,
                onRetry: async (_, _) =>
                {
                    token = await _twitchAccessTokenService.GetTwitchAccessTokenAsync(true);
                }
            );

        var response = await authPolicy.ExecuteAsync(
            () =>
                url.WithHeader("Content-Type", "text/plain")
                    .WithHeader("Client-Id", clientId)
                    .WithOAuthBearerToken(token)
                    .PostAsync(new StringContent(body))
                    .ReceiveString()
        );

        // Workaround, flurl receiving a type directly does not serialize properly
        var deserialize = JsonSerializer.Deserialize<T>(response);
        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(
            TimeSpan.FromDays(30)
        );
        //cacheKey = Guid.NewGuid();
        //_cacheKeys.Add(body, cacheKey);
        //_cache.Set(cacheKey, response, cacheEntryOptions);
        return deserialize;
    }
}
