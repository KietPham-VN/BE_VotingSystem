using System.Net;

namespace BE_VotingSystem.Domain.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string message = "Resource not found", string? errorCode = null)
        : base(message, HttpStatusCode.NotFound, errorCode)
    {
    }
}

public class ValidationException : AppException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors, string message = "Validation failed", string? errorCode = null)
        : base(message, HttpStatusCode.BadRequest, errorCode)
    {
        Errors = errors;
    }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Unauthorized", string? errorCode = null)
        : base(message, HttpStatusCode.Unauthorized, errorCode)
    {
    }
}

public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "Forbidden", string? errorCode = null)
        : base(message, HttpStatusCode.Forbidden, errorCode)
    {
    }
}

public class ConflictException : AppException
{
    public ConflictException(string message = "Conflict", string? errorCode = null)
        : base(message, HttpStatusCode.Conflict, errorCode)
    {
    }
}

public class BadRequestException : AppException
{
    public BadRequestException(string message = "Bad request", string? errorCode = null)
        : base(message, HttpStatusCode.BadRequest, errorCode)
    {
    }
}
