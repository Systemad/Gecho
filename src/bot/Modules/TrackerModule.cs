using bot.Persistence;
using bot.Services;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace bot.Modules;

public class TrackerModule : BaseCommandModule
{
    
    public TrackerService TrackerService { private get; set; }
    
    [Command("track")]
    public async Task GameCommand(CommandContext context, [RemainingText] string name)
    {
        using var gameContext = new GameContext();
        var game = await gameContext.Games.Where(g => g.Name == name).FirstOrDefaultAsync();
        
        var gameEmbed = 
            """
            Game: {game.Name}
            Category: {game.Category}
            Genres: {string.Join(",", game.Genres.Select(x => x.Name))}
            """;
        
        var embedMessage = new DiscordMessageBuilder()
            .AddEmbed(
                new DiscordEmbedBuilder()
                    .WithTitle("Game")
                    .WithDescription(gameEmbed)
                    .WithColor(DiscordColor.Blurple)
            )
            .AddComponents(
                new DiscordButtonComponent(ButtonStyle.Primary, "track_game", "Track game!")
            );

        var message = await context.RespondAsync(embedMessage);
        var interactivity = context.Client.GetInteractivity();
        var result = await interactivity.WaitForButtonAsync(message, context.Member);
        if (!result.TimedOut)
        {
            if (result.Result.Id == "track_game")
            {
                var reminder = await TrackerService.AddUserToTrackedGame(game.Id, context.Member);
                var role = context.Guild.Roles.FirstOrDefault(r => r.Value.Name == reminder.RoleName).Value;
                var newRole = await context.Guild.CreateRoleAsync(name: role.Name, color: new DiscordColor(reminder.RoleColor));
                await context.Member.GrantRoleAsync(newRole);
                await result.Result.Interaction.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("Game tracked!")
                );
            }
        }
    }

    [Command("search"), Aliases("s")]
    public async Task SearchCommand(CommandContext context, [RemainingText] string name)
    {
        await context.RespondAsync("Command executed");
    }

    [Command("what")]
    public async Task ReactionCommand(CommandContext context)
    {
        var builder = new DiscordMessageBuilder()
            .WithContent("This message has buttons! Pretty neat innit?")
            .AddComponents(
                new DiscordButtonComponent(ButtonStyle.Primary, "1_top_d", "Blurple!"),
                new DiscordButtonComponent(ButtonStyle.Secondary, "2_top_d", "Grey!", true),
                new DiscordButtonComponent(ButtonStyle.Success, "3_top_d", "Green!", true),
                new DiscordButtonComponent(ButtonStyle.Danger, "4_top_d", "Red!", true),
                new DiscordLinkButtonComponent("https://some-super-cool.site", "Link!", true)
            );
        var message = await context.RespondAsync(builder);
        var interactivity = context.Client.GetInteractivity();
        var button = await interactivity.WaitForButtonAsync(message);
        await button.Result.Interaction.CreateResponseAsync(
            InteractionResponseType.UpdateMessage,
            new DiscordInteractionResponseBuilder().WithContent("works")
        );
    }

    [Command("woot")]
    public async Task TestWhatCommand(CommandContext context)
    {
        var builder = new DiscordMessageBuilder()
            .WithContent("This message has buttons! Pretty neat innit?")
            .AddComponents(
                new DiscordButtonComponent(ButtonStyle.Primary, "1_top_d", "Blurple!"),
                new DiscordButtonComponent(ButtonStyle.Secondary, "2_top_d", "Grey!"),
                new DiscordButtonComponent(ButtonStyle.Success, "3_top_d", "Green!"),
                new DiscordButtonComponent(ButtonStyle.Danger, "4_top_d", "Red!"),
                new DiscordLinkButtonComponent("https://some-super-cool.site", "Link!", true)
            );

        var message = await context.RespondAsync(builder);
        var interactivity = context.Client.GetInteractivity();
        var result = await interactivity.WaitForButtonAsync(message, context.Member);
        if (!result.TimedOut)
        {
            if (result.Result.Id == "track_game")
            {
                await result.Result.Interaction.CreateResponseAsync(
                    InteractionResponseType.UpdateMessage,
                    new DiscordInteractionResponseBuilder().WithContent("works")
                );
            }
        }
    }
}
