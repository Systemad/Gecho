using bot.Models;
using Microsoft.EntityFrameworkCore;

namespace bot.Persistence;

public class MainDbContext : DbContext
{
    public DbSet<DiscordUser> DiscordUsers { get; set; }
    public DbSet<GameReminder> GameReminders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(
                "Host=localhost;Port=5433;Database=gamewatchdb;Username=postgres;Password=Compaq2009"
            )
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}
