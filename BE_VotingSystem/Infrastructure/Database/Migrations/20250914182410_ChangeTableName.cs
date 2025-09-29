using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_vote_account_AccountId",
                table: "Feedback_vote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Feedback_vote",
                table: "Feedback_vote");

            migrationBuilder.RenameTable(
                name: "Feedback_vote",
                newName: "feedback_vote");

            migrationBuilder.AddPrimaryKey(
                name: "PK_feedback_vote",
                table: "feedback_vote",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_feedback_vote_account_AccountId",
                table: "feedback_vote",
                column: "AccountId",
                principalTable: "account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_feedback_vote_account_AccountId",
                table: "feedback_vote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_feedback_vote",
                table: "feedback_vote");

            migrationBuilder.RenameTable(
                name: "feedback_vote",
                newName: "Feedback_vote");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feedback_vote",
                table: "Feedback_vote",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_vote_account_AccountId",
                table: "Feedback_vote",
                column: "AccountId",
                principalTable: "account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
