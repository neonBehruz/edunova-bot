#nullable enable

using System.Net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StudentAssistant.Bot.Background;

public class HttpHealthCheckWorker : BackgroundService
{
    private readonly ILogger<HttpHealthCheckWorker> _logger;

    public HttpHealthCheckWorker(ILogger<HttpHealthCheckWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string? port = Environment.GetEnvironmentVariable("PORT");
        if (string.IsNullOrEmpty(port))
        {
            _logger.LogInformation("No PORT environment variable specified. Skipping HTTP health check server.");
            return;
        }

        try
        {
            var listener = new HttpListener();
            listener.Prefixes.Add($"http://*:{port}/");
            listener.Start();
            _logger.LogInformation("HTTP Health Check server started on port {Port} for Render Web Service compatibility.", port);

            while (!stoppingToken.IsCancellationRequested)
            {
                var context = await listener.GetContextAsync();
                var response = context.Response;
                string responseString = "🎓 EduNova Telegram Bot is running 24/7!";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                response.ContentType = "text/plain; charset=utf-8";
                using var output = response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length, stoppingToken);
                response.StatusCode = 200;
                response.Close();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HTTP Health Check server error.");
        }
    }
}
