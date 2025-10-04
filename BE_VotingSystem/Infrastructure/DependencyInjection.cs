using BE_VotingSystem.Application.Interfaces.Services;
using BE_VotingSystem.Application.Interfaces.Utils;
using BE_VotingSystem.Domain.Entities;
using BE_VotingSystem.Extensions;
using BE_VotingSystem.Infrastructure.Services;

namespace BE_VotingSystem.Infrastructure;

/// <summary>
///     Provides extension methods for configuring infrastructure services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    ///     Adds all infrastructure services to the service collection
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configuration">The application configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseConfiguration(configuration);
        services.AddAuthenticationConfiguration(configuration);
        services.AddGoogleAuthConfiguration(configuration);
        services.AddHangfireConfiguration(configuration);
        services.AddAuthorization();
        services.AddAuthorizationPolicies();

        services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IExternalAuthCallbackService, ExternalAuthCallbackService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IResetVotesService, ResetVotesService>();
        services.AddScoped<ILecturerService, LecturerService>();
        services.AddScoped<IFeedbackVoteService, FeedbackVoteService>();
        services.AddScoped<ILectureVoteService, LectureVoteService>();
        services.AddScoped<IWebImageService, WebImageService>();
        services.AddScoped<IReportService, ReportService>();
        return services;
    }
}