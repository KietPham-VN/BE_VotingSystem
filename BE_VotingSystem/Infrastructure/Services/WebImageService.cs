using BE_VotingSystem.Application.Dtos.WebImage;
using BE_VotingSystem.Application.Interfaces;
using BE_VotingSystem.Application.Interfaces.Services;
using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Infrastructure.Services;

/// <summary>
///     Service implementation for managing web image entries.
/// </summary>
public class WebImageService(IAppDbContext context) : IWebImageService
{
    /// <inheritdoc />
    public async Task<IEnumerable<WebImageDto>> GetAllImages(CancellationToken cancellationToken = default)
    {
        return await context.WebImages
            .AsNoTracking()
            .Select(wi => new WebImageDto
                (
                    wi.Name ?? string.Empty,
                    wi.ImageUrl ?? string.Empty
                )
            ).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WebImageDto?> GetImageByName(string name, CancellationToken cancellationToken = default)
    {
        var normalized = name.Trim();

        return await context.WebImages
            .AsNoTracking()
            .Where(wi => wi.Name != null && EF.Functions.Collate(wi.Name, "utf8mb4_unicode_ci") == EF.Functions.Collate(normalized, "utf8mb4_unicode_ci"))
            .Select
            (wi => new WebImageDto
                (
                    wi.Name ?? string.Empty,
                    wi.ImageUrl ?? string.Empty
                )
            )
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WebImageDto> CreateImage(WebImageDto webImageDto, CancellationToken cancellationToken = default)
    {
        var name = webImageDto.Name.Trim();
        var imageUrl = webImageDto.ImageUrl.Trim();

        var exists = await context.WebImages
            .AsNoTracking()
            .AnyAsync(wi => wi.Name != null && EF.Functions.Collate(wi.Name, "utf8mb4_unicode_ci") == EF.Functions.Collate(name, "utf8mb4_unicode_ci"),
                cancellationToken);

        if (exists)
            throw new InvalidOperationException($"Image with name '{name}' already exists.");

        await context.WebImages.AddAsync(new WebImage
        {
            Name = name,
            ImageUrl = imageUrl
        }, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return new WebImageDto(name, imageUrl);
    }

    /// <inheritdoc />
    public async Task<WebImageDto> UpdateImage(WebImageDto webImageDto, CancellationToken cancellationToken = default)
    {
        var name = webImageDto.Name.Trim();
        var image = await context.WebImages.FirstOrDefaultAsync(wi =>
            wi.Name != null && EF.Functions.Collate(wi.Name, "utf8mb4_unicode_ci") == EF.Functions.Collate(name, "utf8mb4_unicode_ci"), cancellationToken);
        if (image is null) throw new KeyNotFoundException("Image not found");

        // Do not change primary key (Name). Only update URL.
        image.ImageUrl = webImageDto.ImageUrl.Trim();
        await context.SaveChangesAsync(cancellationToken);
        return new WebImageDto(image.Name ?? string.Empty, image.ImageUrl ?? string.Empty);
    }

    /// <inheritdoc />
    public async Task<WebImageDto> DeleteImage(string name, CancellationToken cancellationToken = default)
    {
        var normalized = name.Trim();
        var image = await context.WebImages.FirstOrDefaultAsync(wi =>
            wi.Name != null && EF.Functions.Collate(wi.Name, "utf8mb4_unicode_ci") == EF.Functions.Collate(normalized, "utf8mb4_unicode_ci"), cancellationToken);
        if (image is null) throw new KeyNotFoundException("Image not found");

        context.WebImages.Remove(image);
        await context.SaveChangesAsync(cancellationToken);
        return new WebImageDto(image.Name ?? string.Empty, image.ImageUrl ?? string.Empty);
    }
}