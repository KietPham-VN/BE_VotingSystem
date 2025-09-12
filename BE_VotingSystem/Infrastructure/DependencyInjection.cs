using System.Text;
using System.Text.Json;
using BE_VotingSystem.Application.Interfaces;
using BE_VotingSystem.Application.Interfaces.Services;
using BE_VotingSystem.Application.Interfaces.Utils;
using BE_VotingSystem.Domain.Entities;
using BE_VotingSystem.Infrastructure.Database;
using BE_VotingSystem.Infrastructure.Services;
using BE_VotingSystem.Infrastructure.Setting;
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BE_VotingSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseSettings>(
            configuration.GetSection(DatabaseSettings.SectionName));

        var dbSettings = configuration
            .GetSection(DatabaseSettings.SectionName)
            .Get<DatabaseSettings>();

        if (dbSettings == null || string.IsNullOrEmpty(dbSettings.DefaultConnection))
            throw new InvalidOperationException(" Missing connection string in appsettings.json");

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                dbSettings.DefaultConnection,
                ServerVersion.AutoDetect(dbSettings.DefaultConnection)
            )
        );

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        var jwt = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
                  ?? throw new InvalidOperationException("Missing Jwt settings");

        // Google settings
        services.Configure<GoogleSettings>(configuration.GetSection(GoogleSettings.SectionName));
        var google = configuration.GetSection(GoogleSettings.SectionName).Get<GoogleSettings>();

        services
            .AddCookiePolicy(options => { options.MinimumSameSitePolicy = SameSiteMode.Lax; })
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Challenge with 401 for APIs
            })
            .AddJwtBearer(options =>
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
            })
            // cookie tạm cho external sign-in
            .AddCookie("External", cookie =>
            {
                cookie.Cookie.SameSite = SameSiteMode.Lax;
                cookie.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // ✅ Sửa ở đây
            })
            // Google OAuth
            .AddGoogle("Google", options =>
            {
                if (google is null) throw new InvalidOperationException("Missing Google settings");
                options.ClientId = google.ClientId;
                options.ClientSecret = google.ClientSecret;
                options.CallbackPath = google.CallbackPath; // "/signin-google"
                options.SignInScheme = "External";
                options.SaveTokens = true;
                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // ✅ Sửa ở đây
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

        // Hangfire configuration
        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseStorage(
                new MySqlStorage(
                    dbSettings.DefaultConnection,
                    new MySqlStorageOptions
                    {
                        QueuePollInterval = TimeSpan.FromSeconds(10),
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),
                        PrepareSchemaIfNecessary = true,
                        DashboardJobListLimit = 25000,
                        TransactionTimeout = TimeSpan.FromMinutes(1),
                        TablesPrefix = "Hangfire",
                    }
                )
            ));

        services.AddHangfireServer(options => options.WorkerCount = 1);

        services.AddAuthorization();

        services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IExternalAuthCallbackService, ExternalAuthCallbackService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IResetVotesService, ResetVotesService>();
        return services;
    }
}