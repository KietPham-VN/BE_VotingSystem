using BE_VotingSystem.Domain.Exceptions;

namespace BE_VotingSystem.Api.Middlewares;

/// <summary>
///     Global exception handling middleware for the application
/// </summary>
public class GlobalExceptionMiddleware(RequestDelegate next)
{
    /// <summary>
    ///     Invokes the middleware to handle exceptions
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Guid.NewGuid().ToString();
        context.Items["CorrelationId"] = correlationId;

        Log.Information("Request started: {Method} {Path} from {RemoteIp} - CorrelationId: {CorrelationId}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress,
            correlationId);

        try
        {
            await next(context).ConfigureAwait(false);

            Log.Information("Request completed: {Method} {Path} - Status: {StatusCode} - CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                correlationId);
        }
        catch (AppException appEx)
        {
            Log.Warning(appEx,
                "Handled AppException: {ExceptionType} - Method: {Method} - Path: {Path} - CorrelationId: {CorrelationId}",
                appEx.GetType().Name,
                context.Request.Method,
                context.Request.Path,
                correlationId);

            IDictionary<string, string[]>? errors = null;
            if (appEx is Domain.Exceptions.ValidationException vex) errors = vex.Errors;

            await WriteProblemDetailsAsync(
                context,
                (int)appEx.StatusCode,
                appEx.GetType().Name,
                appEx.Message,
                errors,
                correlationId).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Handle request cancellation (client disconnect, timeout, etc.)
            Log.Information(
                "Request cancelled: {Method} {Path} - CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                correlationId);

            // Don't write response if the request was cancelled
            if (!context.RequestAborted.IsCancellationRequested)
            {
                await WriteProblemDetailsAsync(
                    context,
                    499, // Client Closed Request status code
                    "Request Cancelled",
                    "The request was cancelled by the client.",
                    null,
                    correlationId).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex,
                "Unhandled exception - Method: {Method} - Path: {Path} - UserAgent: {UserAgent} - CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                context.Request.Headers.UserAgent,
                correlationId);

            await WriteProblemDetailsAsync(
                context,
                500,
                "Internal Server Error",
                "An unexpected error occurred.",
                null,
                correlationId);
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
    /// <param name="correlationId">Correlation ID for tracking</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private static async Task WriteProblemDetailsAsync(HttpContext context, int statusCode, string title, string detail,
        IDictionary<string, string[]>? errors = null, string? correlationId = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new
        {
            title,
            status = statusCode,
            detail,
            errors,
            correlationId,
            timestamp = DateTime.UtcNow,
            path = context.Request.Path,
            method = context.Request.Method
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails), context.RequestAborted)
            .ConfigureAwait(false);
    }
}