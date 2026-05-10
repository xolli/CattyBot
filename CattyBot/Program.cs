using CattyBot.background;
using CattyBot.buttons;
using CattyBot.database;
using CattyBot.handlers;
using CattyBot.handlers.commands;
using CattyBot.services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Telegram.Bot;

namespace CattyBot;

public static class Program
{
    private const string LogDirectoryEnv = "CATTY_LOG_DIRECTORY";

    private const string LogFilename = "catty.log";

    private const string TelegramEnv = "TELEGRAM_BOT_TOKEN";

    private const string OpenAiTokenEnv = "OPENAI_TOKEN";

    public static void Main()
    {
        CreateHostBuilder().Build().Run();
    }

    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                SetLogger();
                services.AddDbContext<CattyBotContext>(options =>
                {
                    options.UseNpgsql(CattyBotContext.GetConnectionString());
                });
                Log.Information("Application have been started");
                services.AddScoped<UserService>();
                services.AddScoped<EventService>();
                services.AddScoped<MessageService>();
                services.AddScoped<StatsService>();
                services.AddScoped<ResponseConfigService>();
                services.AddScoped<SystemPromptService>();
                services.AddScoped<AnalyticsService>();
                services.AddScoped<BirthdaysService>();
                services.AddScoped<LocaleService>();
                services.AddScoped<ReactionsService>();
                services.AddScoped<CommandFactory>();
                services.AddScoped<GetStatsCommand>();
                services.AddScoped<GetAnalyticsCommand>();
                services.AddScoped<GetBirthdaysCommand>();
                services.AddScoped<ClearContextCommand>();
                services.AddScoped<SetModeCommand>();
                services.AddScoped<SetBirthdayCommand>();
                services.AddScoped<ForceSetBirthdayCommand>();
                services.AddScoped<GetReactionsStatisticsCommand>();
                services.AddScoped<GetStatsPieCommand>();
                services.AddScoped<GetStatsChartCommand>();
                services.AddScoped<ReverseHelloMessageConfigCommand>();
                services.AddScoped<ReverseChatBotCommand>();
                services.AddScoped<ReactionHandler>();
                services.AddScoped<RemoveBirthday>();
                services.AddScoped<DeactivateInactiveChats>();
                services.AddScoped<SysPromptAddCommand>();
                services.AddScoped<SysPromptListCommand>();
                services.AddScoped<SysPromptEditCommand>();
                services.AddScoped<SysPromptUpdateCommand>();
                services.AddScoped<SysPromptDeleteCommand>();
                SetTelegramClient(services);
                services.AddScoped<CallbackActionFactory>();
                services.AddScoped<GeminiHandler>();
                services.AddScoped<OpenRouterHandler>();
                services.AddHostedService<CattyBotService>();
                services.AddHostedService<BirthdaysNotifier>();
            })
            .ConfigureLogging((context, logging) =>
            {
                var config = context.Configuration.GetSection("Logging");
                logging.AddConfiguration(config);
                logging.AddConsole();
                logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
            });
    }

    private static void SetTelegramClient(IServiceCollection services)
    {
        var token = Environment.GetEnvironmentVariable(TelegramEnv);
        if (token == null)
            throw new EnvVariablesException($"Expect Telegram token. Set it to environment variable {TelegramEnv}");
        services.AddSingleton(new TelegramBotClient(token));
    }

    private static void SetLogger()
    {
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console();
        var logDirectory = Environment.GetEnvironmentVariable(LogDirectoryEnv);
        if (logDirectory != null)
        {
            Directory.CreateDirectory(logDirectory);
            loggerConfiguration = loggerConfiguration.WriteTo.File(Path.Combine(logDirectory, LogFilename),
                rollingInterval: RollingInterval.Day);
        }

        Log.Logger = loggerConfiguration.CreateLogger();
    }
}
