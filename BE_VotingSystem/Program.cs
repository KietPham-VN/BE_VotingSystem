using BE_VotingSystem.Api.Middlewares;
using BE_VotingSystem.Infrastructure;
using BE_VotingSystem.Infrastructure.Extensions;
using BE_VotingSystem.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BE_VotingSystem API", Version = "v1" });

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
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

// Tắt debug log cho authentication
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Warning);

_ = builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();


// Setup recurring jobs
app.Services.RegisterRecurringJobs();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax,
    Secure = CookieSecurePolicy.SameAsRequest // Quan trọng cho localhost
});

app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new HangfireAuthorizationFilter()],
    IgnoreAntiforgeryToken = true
});

// Global exception middleware sau authentication để có thể access user context
app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

await app.RunAsync();
