using System.Text;
using BE_VotingSystem.Infrastructure.Setting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace BE_VotingSystem.Extensions;

/// <summary>
///     Provides extension methods for configuring authentication services
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    ///     Configures JWT authentication with the provided configuration
    /// </summary>
    /// <param name="authBuilder">The authentication builder</param>
    /// <param name="configuration">The application configuration</param>
    /// <returns>The authentication builder for chaining</returns>
    public static AuthenticationBuilder AddJwtConfiguration(this AuthenticationBuilder authBuilder,
        IConfiguration configuration)
    {
        var jwt = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
                  ?? throw new InvalidOperationException("Missing Jwt settings");

        authBuilder.AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = jwt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        return authBuilder;
    }

    /// <summary>
    ///     Adds authentication configuration to the service collection
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configuration">The application configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtConfiguration(configuration)
            .AddGoogleAuthConfiguration(configuration);

        return services;
    }

    /// <summary>
    ///     Adds authorization policies including admin-only access
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                {
                    var isAdminClaim = context.User.FindFirst("isAdmin");
                    return isAdminClaim?.Value == "True";
                });
            });
        });

        return services;
    }
}