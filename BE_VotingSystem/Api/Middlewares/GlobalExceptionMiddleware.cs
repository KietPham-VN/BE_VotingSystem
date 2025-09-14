using System.Text.Json;
using BE_VotingSystem.Domain.Exceptions;

namespace BE_VotingSystem.Api.Middlewares;

/// <summary>
///     Global exception handling middleware for the application
/// </summary>
public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    /// <summary>
    ///     Invokes the middleware to handle exceptions
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context).ConfigureAwait(false);
        }
        catch (AppException appEx)
        {
            logger.LogWarning(appEx, "Handled AppException: {ExceptionType}", appEx.GetType().Name);

            IDictionary<string, string[]>? errors = null;
            if (appEx is ValidationException vex) errors = vex.Errors;

            await WriteProblemDetailsAsync(
                context,
                (int)appEx.StatusCode,
                appEx.GetType().Name,
                appEx.Message,
                errors).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteProblemDetailsAsync(context, 500, "Internal Server Error", "An unexpected error occurred.");
        }
    }

    /// <summary>
    ///     Writes problem details to the HTTP response
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <param name="statusCode">The HTTP status code</param>
    /// <param name="title">The problem title</param>
    /// <param name="detail">The problem detail</param>
    /// <param name="errors">Optional validation errors</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private static async Task WriteProblemDetailsAsync(HttpContext context, int statusCode, string title, string detail,
        IDictionary<string, string[]>? errors = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new
        {
            title,
            status = statusCode,
            detail,
            errors
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails), context.RequestAborted)
            .ConfigureAwait(false);
    }
}