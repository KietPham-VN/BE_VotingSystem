using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTableNametest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_FeedbackVotes_VotedAt",
                table: "feedback_vote",
                newName: "IX_feedback_vote_VotedAt");

            migrationBuilder.RenameIndex(
                name: "IX_FeedbackVotes_AccountId",
                table: "feedback_vote",
                newName: "IX_feedback_vote_AccountId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "VotedAt",
                table: "feedback_vote",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)",
                comment: "Date and time when the vote was cast",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValueSql: "CURRENT_TIMESTAMP(6)");

            migrationBuilder.AlterColumn<int>(
                name: "Vote",
                table: "feedback_vote",
                type: "int",
                nullable: false,
                comment: "Vote value between 1 and 5 (star rating)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountId",
                table: "feedback_vote",
                type: "char(36)",
                nullable: false,
                comment: "Unique identifier of the account that cast the vote",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_feedback_vote_VotedAt",
                table: "feedback_vote",
                newName: "IX_FeedbackVotes_VotedAt");

            migrationBuilder.RenameIndex(
                name: "IX_feedback_vote_AccountId",
                table: "feedback_vote",
                newName: "IX_FeedbackVotes_AccountId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "VotedAt",
                table: "feedback_vote",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValueSql: "CURRENT_TIMESTAMP(6)",
                oldComment: "Date and time when the vote was cast");

            migrationBuilder.AlterColumn<int>(
                name: "Vote",
                table: "feedback_vote",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Vote value between 1 and 5 (star rating)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountId",
                table: "feedback_vote",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Unique identifier of the account that cast the vote")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");
        }
    }
}
