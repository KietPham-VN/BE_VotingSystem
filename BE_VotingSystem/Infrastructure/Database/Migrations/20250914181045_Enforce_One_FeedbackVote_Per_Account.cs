using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Enforce_One_FeedbackVote_Per_Account : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Feedback_vote",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Vote = table.Column<int>(type: "int", nullable: false),
                    VotedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback_vote", x => x.AccountId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackVotes_AccountId",
                table: "Feedback_vote",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackVotes_VotedAt",
                table: "Feedback_vote",
                column: "VotedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedback_vote");
        }
    }
}
