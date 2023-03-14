using bot.Helpers;
using bot.Models;
using bot.Persistence;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using DiscordUser = bot.Models.DiscordUser;

namespace bot.Services;

public class TrackerService
{
    private readonly MainDbContext _context;

    public TrackerService(MainDbContext context)
    {
        _context = context;
    }

    public async Task<GameReminder> AddUserToTrackedGame(long gameId, DiscordMember discordMember)
    {
        var reminder = await CreateOrGetReminderAsync(gameId);
        var user = new DiscordUser { Id = discordMember.Id, Username = discordMember.Username };
        reminder.DiscordUsers.Add(user);
        user.TrackedGames.Add(reminder);
        await _context.SaveChangesAsync();
        return reminder;
    }

    private async Task<GameReminder> CreateOrGetReminderAsync(long gameId)
    {
        var reminder = await _context.GameReminders.FirstOrDefaultAsync(x => x.GameId == gameId);

        if (reminder is not null)
            return reminder;

        var gameContext = new GameContext();
        var game = await gameContext.Games.FirstOrDefaultAsync(x => x.Id == gameId);
        var newRoleName = game.Name.CreateRoleNameForGame();
        var newReminder = new GameReminder
        {
            Id = Guid.NewGuid(),
            RoleId = new Guid(),
            RoleName = newRoleName,
            RoleColor = DiscordColor.Aquamarine.Value, // TODO: Set random color
            GameId = gameId,
            DiscordUsers = new List<DiscordUser>()
        };
        return newReminder;
    }
}
