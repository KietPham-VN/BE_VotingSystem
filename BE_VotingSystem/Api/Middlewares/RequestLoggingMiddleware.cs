namespace BE_VotingSystem.Api.Middlewares;

/// <summary>
/// Middleware for logging HTTP requests and responses
/// </summary>
public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    /// <summary>
    /// Invokes the middleware to log request details
    /// </summary>
    /// <param name="context">The HTTP context</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
        
        logger.LogInformation(
            "HTTP Request: {Method} {Path} from {RemoteIp} - UserAgent: {UserAgent} - CorrelationId: {CorrelationId}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress,
            context.Request.Headers.UserAgent,
            correlationId);

        if (context.Request.Method is "POST" or "PUT" or "PATCH")
        {
            var contentType = context.Request.ContentType;
            var contentLength = context.Request.ContentLength;
            
            logger.LogInformation("Request details - ContentType: {ContentType}, ContentLength: {ContentLength}, HasFormContentType: {HasFormContentType} - CorrelationId: {CorrelationId}",
                contentType,
                contentLength,
                context.Request.HasFormContentType,
                correlationId);
            
            if (contentType?.Contains("multipart/form-data") == true)
            {
                logger.LogInformation("File upload detected - ContentType: {ContentType}, ContentLength: {ContentLength} - CorrelationId: {CorrelationId}",
                    contentType,
                    contentLength,
                    correlationId);
                
                try
                {
                    if (context.Request.HasFormContentType)
                    {
                        var form = await context.Request.ReadFormAsync();
                        logger.LogInformation("Form fields count: {FieldCount} - CorrelationId: {CorrelationId}",
                            form.Count,
                            correlationId);
                        
                        foreach (var field in form)
                        {
                            if (field.Value.Count > 0)
                            {
                                logger.LogInformation("Form field: {FieldName} = {FieldValue} - CorrelationId: {CorrelationId}",
                                    field.Key,
                                    field.Value.ToString(),
                                    correlationId);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to read form data - CorrelationId: {CorrelationId}", correlationId);
                }
            }
            else
            {
                try
                {
                    context.Request.EnableBuffering();
                    var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
                    context.Request.Body.Position = 0;
                    
                    if (body.Length < 1000)
                    {
                        logger.LogDebug("Request body: {Body} - CorrelationId: {CorrelationId}", body, correlationId);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to read request body - CorrelationId: {CorrelationId}", correlationId);
                }
            }
        }

        try
        {
            await next(context);
            
            var totalDuration = DateTime.UtcNow - startTime;
            logger.LogInformation(
                "HTTP Response: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms - CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                totalDuration.TotalMilliseconds,
                correlationId);
        }
        catch (Exception ex)
        {
            var duration = DateTime.UtcNow - startTime;
            logger.LogError(ex, 
                "Request failed after {Duration}ms - Method: {Method} - Path: {Path} - Status: {StatusCode} - CorrelationId: {CorrelationId}",
                duration.TotalMilliseconds,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                correlationId);
            
            throw;
        }
        finally
        {
            try
            {
                var finalDuration = DateTime.UtcNow - startTime;
                if (finalDuration.TotalSeconds > 30)
                {
                    logger.LogWarning("Slow request detected: {Method} {Path} - Duration: {Duration}ms - CorrelationId: {CorrelationId}",
                        context.Request.Method,
                        context.Request.Path,
                        finalDuration.TotalMilliseconds,
                        correlationId);
                }
            }
            catch (Exception cleanupEx)
            {
                logger.LogError(cleanupEx, "Error in request cleanup - CorrelationId: {CorrelationId}", correlationId);
            }
        }
    }
}
