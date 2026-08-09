using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentAssistant.Data.Context;

namespace StudentAssistant.Data.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddStudentAssistantDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        // Environment variables override appsettings (common in cloud hosts like Render, Koyeb, Supabase, Neon)
        string connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                                  ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
                                  ?? configuration.GetConnectionString("DefaultConnection") 
                                  ?? "Data Source=student_assistant.db";

        string provider = configuration["DatabaseProvider"]?.ToLowerInvariant() ?? "";

        bool isPostgres = provider == "postgres" 
                          || provider == "postgresql" 
                          || connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase)
                          || connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)
                          || connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase)
                          || connectionString.Contains("Server=", StringComparison.OrdinalIgnoreCase);

        if (isPostgres)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            string pgConnectionString = ParsePostgresUrlToConnectionString(connectionString);
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(pgConnectionString, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
        }
        else
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connectionString, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
        }

        return services;
    }

    private static string ParsePostgresUrlToConnectionString(string rawConnectionString)
    {
        if (!rawConnectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) && 
            !rawConnectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return rawConnectionString;
        }

        var uri = new Uri(rawConnectionString);
        var userInfo = uri.UserInfo.Split(':', 2);
        var username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : "";
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 5432;
        var database = uri.AbsolutePath.TrimStart('/');

        return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
    }
}
