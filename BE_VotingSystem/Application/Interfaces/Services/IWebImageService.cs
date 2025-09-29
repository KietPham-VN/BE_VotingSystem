using BE_VotingSystem.Application.Dtos.WebImage;

namespace BE_VotingSystem.Application.Interfaces.Services;

/// <summary>
///     Contract for managing web image entries.
/// </summary>
public interface IWebImageService
{
    /// <summary>
    ///     Retrieves all web images.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<IEnumerable<WebImageDto>> GetAllImages(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a single web image by its unique name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<WebImageDto?> GetImageByName(string name, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new web image entry.
    /// </summary>
    /// <param name="webImageDto"></param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<WebImageDto> CreateImage(WebImageDto webImageDto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an existing web image entry (URL only).
    /// </summary>
    /// <param name="webImageDto"></param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<WebImageDto> UpdateImage(WebImageDto webImageDto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a web image by name and returns the deleted entry.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<WebImageDto> DeleteImage(string name, CancellationToken cancellationToken = default);
}