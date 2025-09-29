using BE_VotingSystem.Domain.Entities;

namespace BE_VotingSystem.Infrastructure.Database.Configurations;

/// <summary>
///     Schema definition for WebImage
/// </summary>
public class WebImageConfiguration : IEntityTypeConfiguration<WebImage>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<WebImage> builder)
    {
        builder.ToTable("web_image");

        builder.HasKey(wi => wi.Name);
        builder.Property(wi => wi.Name).HasMaxLength(255);
        builder.Property(wi => wi.ImageUrl).IsRequired();
    }
}