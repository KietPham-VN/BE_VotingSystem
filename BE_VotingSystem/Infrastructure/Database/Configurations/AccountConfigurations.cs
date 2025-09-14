using System.Diagnostics.CodeAnalysis;
using BE_VotingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BE_VotingSystem.Infrastructure.Database.Configurations;

/// <summary>
/// Entity Framework configuration for Account entity
/// </summary>
public class AccountConfigurations : IEntityTypeConfiguration<Account>
{
    /// <summary>
    /// Configures the Account entity for Entity Framework
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    [SuppressMessage("SonarAnalyzer.CSharp", "S2325",
        Justification = "EF Core requires instance method for IEntityTypeConfiguration<T>.")]
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("account", t =>
        {
            t.HasCheckConstraint(
                "CK_Account_StudentCode_Format",
                "(StudentCode IS NULL OR UPPER(StudentCode) REGEXP '^(SS|SA|SE|CS|CA|CE|HS|HE|HA|QS|QA|QE|DS|DA|DE)[0-9]{6}$')");

            t.HasCheckConstraint(
                "CK_Account_Semester_Range",
                "(Semester IS NULL OR (Semester >= 0 AND Semester <= 9))");
        });

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .ValueGeneratedOnAdd();

        builder.Property(a => a.Email)
            .IsRequired()
            .HasMaxLength(255);
        builder.HasIndex(a => a.Email).IsUnique();

        builder.Property(a => a.Name).HasMaxLength(255);

        builder.Property(a => a.VotesRemain).HasDefaultValue(3).IsRequired();

        builder.Property(a => a.Provider)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(a => new { a.Provider, a.ProviderId }).IsUnique();

        builder.Property(a => a.ProviderId).HasMaxLength(255);

        builder.Property(a => a.StudentCode)
            .HasMaxLength(8)
            .IsRequired(false);

        builder.Property(a => a.Semester)
            .HasColumnType("tinyint unsigned")
            .IsRequired(false);

        builder.Property(a => a.Department)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(a => a.IsAdmin)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(a => a.IsBanned)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(a => a.BanReason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(a => a.PasswordHash)
            .HasMaxLength(255);

        builder
            .HasMany(a => a.RefreshTokens)
            .WithOne(rt => rt.Account)
            .HasForeignKey(rt => rt.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(a => a.Votes)
            .WithOne(lv => lv.Account)
            .HasForeignKey(lv => lv.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
