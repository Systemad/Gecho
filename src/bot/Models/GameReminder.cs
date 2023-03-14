using bot.GameDatabase.Models;

namespace bot.Models;

public class GameReminder
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public string RoleName { get; set; }
    public int RoleColor { get; set; }
    public long GameId { get; set; }
    public Game Game { get; set; }
    public List<DiscordUser> DiscordUsers { get; set; }
}
