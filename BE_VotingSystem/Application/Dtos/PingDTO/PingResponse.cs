namespace BE_VotingSystem.Application.Dtos.PingDTO;

/// <summary>
///     Response object for ping/health check operations
/// </summary>
/// <param name="Message">Ping response message</param>
/// <param name="Time">Current UTC time when the ping was processed</param>
public record PingResponse(string Message, DateTime Time);