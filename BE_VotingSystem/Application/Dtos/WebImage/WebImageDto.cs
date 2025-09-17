namespace BE_VotingSystem.Application.Dtos.WebImage;

/// <summary>
///     Data transfer object representing a web image entry.
/// </summary>
/// <param name="Name">Unique name (primary key) of the image.</param>
/// <param name="ImageUrl">Absolute or relative URL where the image is hosted.</param>
public sealed record WebImageDto(string Name, string ImageUrl);
