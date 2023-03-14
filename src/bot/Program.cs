using System.Collections.Specialized;
using bot;
using bot.Modules;
using bot.Services;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Serilog;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

ILogger logger = new LoggerConfiguration().MinimumLevel
    .Information()
    .WriteTo.Console()
    .CreateLogger();

/*
await using var context = new GameContext();
await context.Database.EnsureDeletedAsync();
await context.Database.EnsureCreatedAsync();
*/
var settings = config.GetRequiredSection("Discord").Get<DiscordOptions>();

var discord = new DiscordClient(
    new DiscordConfiguration
    {
        Token = settings.Token,
        TokenType = TokenType.Bot,
        LoggerFactory = new Microsoft.Extensions.Logging.LoggerFactory().AddSerilog(logger),
        Intents = DiscordIntents.All
    }
);
var services = new ServiceCollection().AddSingleton<TrackerService>().BuildServiceProvider();

discord.UseInteractivity(
    new InteractivityConfiguration
    {
        PollBehaviour = PollBehaviour.KeepEmojis,
        Timeout = TimeSpan.FromSeconds(30)
    }
);

var commands = discord.UseCommandsNext(
    new CommandsNextConfiguration { StringPrefixes = new[] { "!" }, Services = services }
);

commands.RegisterCommands<GameModule>();
commands.RegisterCommands<TrackerModule>();
await discord.ConnectAsync();

/*
var properties = new NameValueCollection();

var scheduler = await SchedulerBuilder
    .Create(properties)
    .UseDefaultThreadPool(x => x.MaxConcurrency = 5)
    .UsePersistentStore(
        x =>
        {
            x.UseProperties = true;
            x.UseClustering();
            x.UsePostgres("my connection string");
            x.UseJsonSerializer();
        }
    )
    .BuildScheduler();

await scheduler.Start();
*/
logger.ForContext<Program>().Information("Bot ready");
await Task.Delay(-1);
