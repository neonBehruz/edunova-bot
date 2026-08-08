using Microsoft.EntityFrameworkCore;
using StudentAssistant.Data.Context;
using StudentAssistant.Data.Interfaces;
using StudentAssistant.Data.Repositories;
using StudentAssistant.Service.Interfaces;
using StudentAssistant.Service.Services;
using StudentAssistant.WebApi.Middleware;

namespace StudentAssistant.WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi();

        // Database context
        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=student_assistant.db";
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        // Repositories
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Domain & Application Services
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ISubjectService, SubjectService>();
        builder.Services.AddScoped<IQuestionService, QuestionService>();
        builder.Services.AddScoped<IQuestionGeneratorService, QuestionGeneratorService>();
        builder.Services.AddScoped<ITestService, TestService>();
        builder.Services.AddScoped<ITestAttemptService, TestAttemptService>();
        builder.Services.AddScoped<IProgressService, ProgressService>();
        builder.Services.AddScoped<IRatingService, RatingService>();
        builder.Services.AddScoped<IStatisticsService, StatisticsService>();

        // CORS configuration
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });

        var app = builder.Build();

        // Ensure Database Created & Seeded
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }

        // Configure the HTTP request pipeline.
        app.UseMiddleware<ExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseCors("AllowAll");
        app.UseAuthorization();
        app.MapControllers();

        await app.RunAsync();
    }
}
