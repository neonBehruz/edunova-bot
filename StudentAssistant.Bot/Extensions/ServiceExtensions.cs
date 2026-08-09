using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentAssistant.Data.Context;
using StudentAssistant.Data.Extensions;
using StudentAssistant.Data.Interfaces;
using StudentAssistant.Data.Repositories;
using StudentAssistant.Service.Interfaces;
using StudentAssistant.Service.Services;

namespace StudentAssistant.Bot.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // EF Core DbContext (Auto SQLite or PostgreSQL)
        services.AddStudentAssistantDbContext(configuration);

        // Generic Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Application Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IQuestionGeneratorService, QuestionGeneratorService>();
        services.AddScoped<ITestService, TestService>();
        services.AddScoped<ITestAttemptService, TestAttemptService>();
        services.AddScoped<IProgressService, ProgressService>();
        services.AddScoped<IRatingService, RatingService>();
        services.AddScoped<IStatisticsService, StatisticsService>();

        return services;
    }
}
