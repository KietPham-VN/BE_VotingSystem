using BE_VotingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BE_VotingSystem.Infrastructure.Database.Configurations;

/// <summary>
/// Entity Framework configuration for RefreshToken entity
/// </summary>
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    /// <summary>
    /// Configures the RefreshToken entity for Entity Framework
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_token");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(255);
        builder.HasIndex(x => x.Token).IsUnique();

        builder.Property(x => x.Expires)
            .IsRequired();
        builder.HasIndex(x => x.Expires);

        builder.Property(x => x.AccountId)
            .IsRequired();
        builder.HasIndex(x => x.AccountId);

        builder
            .HasOne(x => x.Account)
            .WithMany(a => a.RefreshTokens)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(t => { t.HasCheckConstraint("CK_RefreshToken_Expires_Future", "`Expires` IS NOT NULL"); });
    }
}
