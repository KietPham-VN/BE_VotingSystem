using System.Diagnostics.CodeAnalysis;
using BE_VotingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BE_VotingSystem.Infrastructure.Database.Configurations;

/// <summary>
/// Entity Framework configuration for Lecture entity
/// </summary>
public class LectureConfiguration : IEntityTypeConfiguration<Lecture>
{
    /// <summary>
    /// Configures the Lecture entity for Entity Framework
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    [SuppressMessage("SonarAnalyzer.CSharp", "S2325",
        Justification = "EF Core requires instance method for IEntityTypeConfiguration<T>.")]
    public void Configure(EntityTypeBuilder<Lecture> builder)
    {
        builder.ToTable("lecture", t =>
        {
            t.HasCheckConstraint(
                "CK_Lecture_Name_NotEmpty",
                "CHAR_LENGTH(TRIM(Name)) > 0");

            t.HasCheckConstraint(
                "CK_Lecture_Department_NotEmpty",
                "CHAR_LENGTH(TRIM(Department)) > 0");
        });

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .ValueGeneratedOnAdd();

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(255);
        builder.HasIndex(l => l.Name);

        builder.Property(l => l.Department)
            .IsRequired()
            .HasMaxLength(255);
        builder.HasIndex(l => l.Department);

        builder.Property(l => l.Quote)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(l => l.AvatarUrl)
            .HasMaxLength(500)
            .IsRequired(false);

        builder
            .HasMany(l => l.Votes)
            .WithOne(lv => lv.Lecture)
            .HasForeignKey(lv => lv.LectureId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
