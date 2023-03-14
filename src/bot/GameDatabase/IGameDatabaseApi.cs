namespace bot.GameDatabase;

public interface IGameDatabaseApi
{
    Task<T> GenericFetch<T>(string url, string body);
}
