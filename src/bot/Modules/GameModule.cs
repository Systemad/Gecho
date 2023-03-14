using bot.Persistence;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace bot.Modules;

public class GameModule : BaseCommandModule
{
    [Command("info")]
    public async Task GameCommand(CommandContext context, [RemainingText] string name)
    {
        //using var gameContext = new GameContext();
        //var game = await gameContext.Games.Where(g => g.Name == name).FirstOrDefaultAsync();
        
        var gameEmbed = 
            """
            Game: {game.Name}
            Category: {game.Category}
            Genres: {string.Join(",", game.Genres.Select(x => x.Name))}
            Developer: {string.Join(",", game.InvolvedCompanies.Select(x => x.Company.Name))}
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
                Console.WriteLine(context.User.Username);
                Console.WriteLine(context.Member.Id);
                await result.Result.Interaction.CreateResponseAsync(
                    InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("Game tracked!")
                );
            }
        }
    }
}


/*
        // Discord snowflakes use 2015 (the launch date) as it's epoch for timestamps.
        private static readonly DateTimeOffset DiscordEpoch = new(2015, 1, 1, 0, 0, 0, TimeSpan.Zero);
        */