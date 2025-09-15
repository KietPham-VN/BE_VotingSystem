namespace BE_VotingSystem.Application.Dtos.Common;

/// <summary>
///     Standard API envelope with a human-friendly message
/// </summary>
public sealed record ApiResponse(string Message);

/// <summary>
///     Standard API envelope with data and a human-friendly message
/// </summary>
public sealed record ApiResponse<T>(T Data, string Message);
