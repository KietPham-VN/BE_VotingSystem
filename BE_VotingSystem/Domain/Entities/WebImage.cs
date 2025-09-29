namespace BE_VotingSystem.Domain.Entities;

/// <summary>
///     Store as dictionary with key: name, value: the url
/// </summary>
public sealed class WebImage
{
    /// <summary>
    ///     name of the image
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Url of the image
    /// </summary>
    public string? ImageUrl { get; set; }
}
