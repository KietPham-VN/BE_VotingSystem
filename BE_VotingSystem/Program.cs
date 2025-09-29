using BE_VotingSystem.Api.Middlewares;
using BE_VotingSystem.Infrastructure;
using BE_VotingSystem.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

ExcelPackage.License.SetNonCommercialPersonal("BE_VotingSystem");

AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
{
    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] UNHANDLED EXCEPTION: {args.ExceptionObject}");
    if (args.ExceptionObject is Exception ex)
    {
        Console.WriteLine($"Exception: {ex.GetType().Name} - {ex.Message}");
    }
};

TaskScheduler.UnobservedTaskException += (sender, args) =>
{
    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] UNOBSERVED TASK EXCEPTION: {args.Exception}");
};


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .SetIsOriginAllowed(origin => true);
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BE_VotingSystem API", Version = "v1" });

    c.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token only (without 'Bearer ' prefix)"
    };
    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
                { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    };
    c.AddSecurityRequirement(securityRequirement);
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024;
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024;
});
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Warning);

_ = builder.Services.AddInfrastructure(builder.Configuration);
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

var app = builder.Build();

try
{
    app.Services.RegisterRecurringJobs();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogCritical(ex, "Application failed to start due to startup exception");
    Environment.Exit(1);
}

if (app.Environment.IsDevelopment())
{
    
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BE_VotingSystem API v1");
        c.RoutePrefix = string.Empty;
        c.DocumentTitle = "BE_VotingSystem API";
        c.DefaultModelsExpandDepth(-1);
        c.DisplayRequestDuration();
    });
    
    app.UseCors("Development");
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    
    context.Response.Headers["Access-Control-Allow-Origin"] = "*";
    context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS";
    context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization";
    
    await next();
});

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax,
    Secure = CookieSecurePolicy.SameAsRequest
});
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new HangfireAuthorizationFilter()],
    IgnoreAntiforgeryToken = true
});
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

try
{
    await app.SeedDatabaseAsync();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Failed to seed database");
}

try
{
    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Starting application...");
    await app.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] APPLICATION RUN FAILED:");
    Console.WriteLine($"Exception type: {ex.GetType().Name}");
    Console.WriteLine($"Exception message: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    Console.WriteLine($"Inner exception: {ex.InnerException}");
    
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogCritical(ex, "Application terminated unexpectedly");
    Environment.Exit(1);
}
