namespace bot.Models;

public class DiscordUser
{
    public ulong Id { get; set; }
    public string Username { get; set; }
    public List<GameReminder> TrackedGames { get; set; }
}
