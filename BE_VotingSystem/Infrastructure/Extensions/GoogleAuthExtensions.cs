using System.Text.Json;
using BE_VotingSystem.Infrastructure.Setting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace BE_VotingSystem.Infrastructure.Extensions;

public static class GoogleAuthExtensions
{
    public static AuthenticationBuilder AddGoogleAuthConfiguration(this AuthenticationBuilder authBuilder,
        IConfiguration configuration)
    {
        // Google settings
        var google = configuration.GetSection(GoogleSettings.SectionName).Get<GoogleSettings>();
        if (google is null) throw new InvalidOperationException("Missing Google settings");

        // cookie tạm cho external sign-in
        authBuilder.AddCookie("External", cookie =>
        {
            cookie.Cookie.SameSite = SameSiteMode.Lax;
            cookie.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        });

        // Google OAuth
        authBuilder.AddGoogle("Google", options =>
        {
            options.ClientId = google.ClientId;
            options.ClientSecret = google.ClientSecret;
            options.CallbackPath = google.CallbackPath; // "/signin-google"
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

    public static IServiceCollection AddGoogleAuthConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<GoogleSettings>(configuration.GetSection(GoogleSettings.SectionName));
        services.AddCookiePolicy(options => { options.MinimumSameSitePolicy = SameSiteMode.Lax; });

        return services;
    }
}
