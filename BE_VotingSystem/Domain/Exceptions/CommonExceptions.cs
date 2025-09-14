using System.Net;

namespace BE_VotingSystem.Domain.Exceptions;

/// <summary>
///     Exception thrown when a requested resource is not found
/// </summary>
public class NotFoundException : AppException
{
    /// <summary>
    ///     Initializes a new instance of the NotFoundException class
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="errorCode">Optional error code</param>
    public NotFoundException(string message = "Resource not found", string? errorCode = null)
        : base(message, HttpStatusCode.NotFound, errorCode)
    {
    }
}

/// <summary>
///     Exception thrown when validation fails
/// </summary>
public class ValidationException : AppException
{
    /// <summary>
    ///     Initializes a new instance of the ValidationException class
    /// </summary>
    /// <param name="errors">Dictionary of validation errors</param>
    /// <param name="message">Exception message</param>
    /// <param name="errorCode">Optional error code</param>
    public ValidationException(IDictionary<string, string[]> errors, string message = "Validation failed",
        string? errorCode = null)
        : base(message, HttpStatusCode.BadRequest, errorCode)
    {
        Errors = errors;
    }

    /// <summary>
    ///     Dictionary of validation errors by field name
    /// </summary>
    public IDictionary<string, string[]> Errors { get; }
}

/// <summary>
///     Exception thrown when authentication is required but not provided
/// </summary>
public class UnauthorizedException : AppException
{
    /// <summary>
    ///     Initializes a new instance of the UnauthorizedException class
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="errorCode">Optional error code</param>
    public UnauthorizedException(string message = "Unauthorized", string? errorCode = null)
        : base(message, HttpStatusCode.Unauthorized, errorCode)
    {
    }
}

/// <summary>
///     Exception thrown when access is forbidden due to insufficient permissions
/// </summary>
public class ForbiddenException : AppException
{
    /// <summary>
    ///     Initializes a new instance of the ForbiddenException class
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="errorCode">Optional error code</param>
    public ForbiddenException(string message = "Forbidden", string? errorCode = null)
        : base(message, HttpStatusCode.Forbidden, errorCode)
    {
    }
}

/// <summary>
///     Exception thrown when a conflict occurs (e.g., duplicate resource)
/// </summary>
public class ConflictException : AppException
{
    /// <summary>
    ///     Initializes a new instance of the ConflictException class
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="errorCode">Optional error code</param>
    public ConflictException(string message = "Conflict", string? errorCode = null)
        : base(message, HttpStatusCode.Conflict, errorCode)
    {
    }
}

/// <summary>
///     Exception thrown when a bad request is made
/// </summary>
public class BadRequestException : AppException
{
    /// <summary>
    ///     Initializes a new instance of the BadRequestException class
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="errorCode">Optional error code</param>
    public BadRequestException(string message = "Bad request", string? errorCode = null)
        : base(message, HttpStatusCode.BadRequest, errorCode)
    {
    }
}