using System.Text.Json;
using BE_VotingSystem.Domain.Exceptions;

namespace BE_VotingSystem.Api.Middlewares;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
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
            if (appEx is ValidationException vex)
            {
                errors = vex.Errors;
            }

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

    private static async Task WriteProblemDetailsAsync(HttpContext context, int statusCode, string title, string detail, IDictionary<string, string[]>? errors = null)
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
