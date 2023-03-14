namespace bot.GameDatabase.Auth;

public interface ITwitchAccessTokenService
{
    Task<string> GetTwitchAccessTokenAsync(bool unauthorized);
}
