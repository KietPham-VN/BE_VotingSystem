using System.Net;

namespace BE_VotingSystem.Domain.Exceptions;

/// <summary>
///     Base exception class for application-specific exceptions
/// </summary>
public abstract class AppException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the AppException class
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="errorCode">Optional error code</param>
    /// <param name="inner">Inner exception</param>
    protected AppException(string message, HttpStatusCode statusCode, string? errorCode = null, Exception? inner = null)
        : base(message, inner)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>
    ///     HTTP status code associated with this exception
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    ///     Optional error code for this exception
    /// </summary>
    public string? ErrorCode { get; }
}