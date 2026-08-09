using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudentAssistant.Bot.Background;
using StudentAssistant.Bot.Configuration;
using StudentAssistant.Bot.Extensions;
using StudentAssistant.Bot.Handlers;
using StudentAssistant.Bot.Services;
using StudentAssistant.Bot.State;
using StudentAssistant.Data.Context;
using Telegram.Bot;

namespace StudentAssistant.Bot;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                // Application Services & DB Context
                services.AddApplicationServices(configuration);

                // Bot Settings
                var botSettings = configuration.GetSection("BotSettings").Get<BotSettings>() ?? new BotSettings();
                services.AddSingleton(botSettings);

                // Telegram Bot Client
                services.AddSingleton<ITelegramBotClient>(sp => new TelegramBotClient(botSettings.Token));

                // Bot State Manager
                services.AddSingleton<TestSessionManager>();

                // Handlers
                services.AddScoped<StartHandler>();
                services.AddScoped<LevelHandler>();
                services.AddScoped<SubjectHandler>();
                services.AddScoped<DifficultyHandler>();
                services.AddScoped<TestHandler>();
                services.AddScoped<QuestionHandler>();
                services.AddScoped<AnswerHandler>();
                services.AddScoped<ResultHandler>();
                services.AddScoped<RatingHandler>();
                services.AddScoped<AboutHandler>();
                services.AddScoped<SupportHandler>();
                services.AddScoped<MainMenuHandler>();

                // Background Hosted Services
                services.AddHostedService<HttpHealthCheckWorker>();
                services.AddHostedService<QuestionTimerWorker>();
                services.AddHostedService<TelegramBotService>();
            })
            .Build();

        // Ensure database created & seeded
        using (var scope = host.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            try
            {
                logger.LogInformation("Ensuring database is created and seeded...");
                await dbContext.Database.EnsureCreatedAsync();
                logger.LogInformation("Database initialization complete.");

                var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
                var botSettings = scope.ServiceProvider.GetRequiredService<BotSettings>();
                try
                {
                    logger.LogInformation("Setting Telegram Bot Description and Short Description...");
                    await botClient.SetMyDescription("🎓 EduNova — bilimni sinash va rivojlantirish uchun aqlli test bot. A1–C2 darajalar, Easy/Middle/Hard qiyinliklar, vaqtli testlar, natijalar va reyting tizimi.");
                    await botClient.SetMyShortDescription("🎓 Learn. Test. Improve. 🚀");

                    string profilePath = Path.Combine(AppContext.BaseDirectory, "profile.jpg");
                    if (File.Exists(profilePath))
                    {
                        logger.LogInformation("Uploading Bot Profile Photo from {ProfilePath}...", profilePath);
                        using var stream = File.OpenRead(profilePath);
                        var photo = new Telegram.Bot.Types.InputProfilePhotoStatic { Photo = Telegram.Bot.Types.InputFile.FromStream(stream, "profile.jpg") };
                        await botClient.SetMyProfilePhoto(photo);
                        logger.LogInformation("Bot Profile Photo set successfully.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Could not set bot description or profile photo via API.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during database initialization.");
            }
        }

        Console.WriteLine("=================================================");
        Console.WriteLine(" 🎓 STUDENT ASSISTANT TELEGRAM BOT STARTED 🎓 ");
        Console.WriteLine("=================================================");
        await host.RunAsync();
    }
}
