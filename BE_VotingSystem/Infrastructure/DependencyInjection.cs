using BE_VotingSystem.Application.Interfaces.Services;
using BE_VotingSystem.Application.Interfaces.Utils;
using BE_VotingSystem.Domain.Entities;
using BE_VotingSystem.Infrastructure.Extensions;
using BE_VotingSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;

namespace BE_VotingSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseConfiguration(configuration);
        services.AddAuthenticationConfiguration(configuration);
        services.AddGoogleAuthConfiguration(configuration);
        services.AddHangfireConfiguration(configuration);
        services.AddAuthorization();

        services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IExternalAuthCallbackService, ExternalAuthCallbackService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IResetVotesService, ResetVotesService>();
        services.AddScoped<ILecturerService, LecturerService>();
        return services;
    }
}
