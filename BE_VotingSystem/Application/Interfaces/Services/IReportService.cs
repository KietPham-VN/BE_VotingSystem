namespace BE_VotingSystem.Application.Interfaces.Services;

/// <summary>
///     Service interface for generating reports
/// </summary>
public interface IReportService
{
    /// <summary>
    ///     Generates a comprehensive voting report with all data
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Excel file as byte array</returns>
    Task<byte[]> GenerateVotingReportAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Generates a lecturer performance report
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Excel file as byte array</returns>
    Task<byte[]> GenerateLecturerReportAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Generates a feedback summary report
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Excel file as byte array</returns>
    Task<byte[]> GenerateFeedbackReportAsync(CancellationToken cancellationToken = default);
}