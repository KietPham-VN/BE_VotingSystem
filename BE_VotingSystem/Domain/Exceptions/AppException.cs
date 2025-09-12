using System.Net;

namespace BE_VotingSystem.Domain.Exceptions;

public abstract class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? ErrorCode { get; }

    protected AppException(string message, HttpStatusCode statusCode, string? errorCode = null, Exception? inner = null)
        : base(message, inner)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}
