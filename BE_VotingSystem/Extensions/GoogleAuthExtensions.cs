using System.Text.Json;
using BE_VotingSystem.Infrastructure.Setting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace BE_VotingSystem.Extensions;

/// <summary>
///     Provides extension methods for configuring Google authentication services
/// </summary>
public static class GoogleAuthExtensions
{
    /// <summary>
    ///     Configures Google OAuth authentication with the provided configuration
    /// </summary>
    /// <param name="authBuilder">The authentication builder</param>
    /// <param name="configuration">The application configuration</param>
    /// <returns>The authentication builder for chaining</returns>
    public static AuthenticationBuilder AddGoogleAuthConfiguration(this AuthenticationBuilder authBuilder,
        IConfiguration configuration)
    {
        var google = configuration.GetSection(GoogleSettings.SectionName).Get<GoogleSettings>();
        if (google is null) throw new InvalidOperationException("Missing Google settings");

        authBuilder.AddCookie("External", cookie =>
        {
            cookie.Cookie.SameSite = SameSiteMode.Lax;
            cookie.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        });

        authBuilder.AddGoogle("Google", options =>
        {
            options.ClientId = google.ClientId;
            options.ClientSecret = google.ClientSecret;
            options.CallbackPath = google.CallbackPath;
            options.SignInScheme = "External";
            options.SaveTokens = true;
            options.CorrelationCookie.SameSite = SameSiteMode.Lax;
            options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Scope.Add("email");
            options.Scope.Add("profile");

            options.Events = new OAuthEvents
            {
                OnRedirectToAuthorizationEndpoint = context =>
                {
                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                },
                OnRemoteFailure = async context =>
                {
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("Auth.Google");
                    logger.LogError(context.Failure,
                        "Google OAuth remote failure. error={Error} desc={Desc} uri={Uri} state={State}",
                        context.Request.Query["error"].ToString(),
                        context.Request.Query["error_description"].ToString(),
                        context.Request.Query["error_uri"].ToString(),
                        context.Request.Query["state"].ToString());

                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/json";
                    var payload = JsonSerializer.Serialize(new
                    {
                        title = "Validation Failed",
                        status = 400,
                        detail = context.Failure?.Message ?? "Remote login failed"
                    });
                    await context.Response.WriteAsync(payload);
                    context.HandleResponse();
                }
            };
        });

        return authBuilder;
    }

    /// <summary>
    ///     Adds Google authentication configuration to the service collection
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configuration">The application configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddGoogleAuthConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<GoogleSettings>(configuration.GetSection(GoogleSettings.SectionName));
        services.AddCookiePolicy(options => { options.MinimumSameSitePolicy = SameSiteMode.Lax; });

        return services;
    }
}