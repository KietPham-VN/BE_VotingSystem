using System.Diagnostics.CodeAnalysis;
using BE_VotingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BE_VotingSystem.Infrastructure.Database.Configurations;

/// <summary>
///     Entity Framework configuration for LecturerVote entity
/// </summary>
public class LecturerVoteConfiguration : IEntityTypeConfiguration<LecturerVote>
{
    /// <summary>
    ///     Configures the LecturerVote entity for Entity Framework
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    [SuppressMessage("SonarAnalyzer.CSharp", "S2325",
        Justification = "EF Core requires instance method for IEntityTypeConfiguration<T>.")]
    public void Configure(EntityTypeBuilder<LecturerVote> builder)
    {
        builder.ToTable("lecturer_vote");

        builder.HasKey(lv => lv.Id);
        builder.Property(lv => lv.Id)
            .ValueGeneratedOnAdd();

        builder.Property(lv => lv.LectureId)
            .IsRequired();

        builder.Property(lv => lv.AccountId)
            .IsRequired();

        builder.Property(lv => lv.VotedAt)
            .IsRequired()
            .HasDefaultValueSql("CURDATE()")
            .HasColumnType("DATE");

        builder.HasIndex(lv => lv.VotedAt);

        // Foreign key relationships
        builder
            .HasOne(lv => lv.Lecture)
            .WithMany(l => l.Votes)
            .HasForeignKey(lv => lv.LectureId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(lv => lv.Account)
            .WithMany(a => a.Votes)
            .HasForeignKey(lv => lv.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint để ngăn vote trùng lặp trong cùng 1 ngày
        // Cho phép vote lại cho cùng 1 lecture vào ngày khác
        builder.HasIndex(lv => new { lv.LectureId, lv.AccountId, lv.VotedAt })
            .IsUnique();
    }
}
