using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTableNameAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lecture_vote_account_AccountId",
                table: "lecture_vote");

            migrationBuilder.DropForeignKey(
                name: "FK_lecture_vote_lecturer_LectureId",
                table: "lecture_vote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_lecture_vote",
                table: "lecture_vote");

            migrationBuilder.RenameTable(
                name: "lecture_vote",
                newName: "lecturer_vote");

            migrationBuilder.RenameIndex(
                name: "IX_lecture_vote_VotedAt",
                table: "lecturer_vote",
                newName: "IX_lecturer_vote_VotedAt");

            migrationBuilder.RenameIndex(
                name: "IX_lecture_vote_LectureId_AccountId_VotedAt",
                table: "lecturer_vote",
                newName: "IX_lecturer_vote_LectureId_AccountId_VotedAt");

            migrationBuilder.RenameIndex(
                name: "IX_lecture_vote_AccountId",
                table: "lecturer_vote",
                newName: "IX_lecturer_vote_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_lecturer_vote",
                table: "lecturer_vote",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_lecturer_vote_account_AccountId",
                table: "lecturer_vote",
                column: "AccountId",
                principalTable: "account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_lecturer_vote_lecturer_LectureId",
                table: "lecturer_vote",
                column: "LectureId",
                principalTable: "lecturer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lecturer_vote_account_AccountId",
                table: "lecturer_vote");

            migrationBuilder.DropForeignKey(
                name: "FK_lecturer_vote_lecturer_LectureId",
                table: "lecturer_vote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_lecturer_vote",
                table: "lecturer_vote");

            migrationBuilder.RenameTable(
                name: "lecturer_vote",
                newName: "lecture_vote");

            migrationBuilder.RenameIndex(
                name: "IX_lecturer_vote_VotedAt",
                table: "lecture_vote",
                newName: "IX_lecture_vote_VotedAt");

            migrationBuilder.RenameIndex(
                name: "IX_lecturer_vote_LectureId_AccountId_VotedAt",
                table: "lecture_vote",
                newName: "IX_lecture_vote_LectureId_AccountId_VotedAt");

            migrationBuilder.RenameIndex(
                name: "IX_lecturer_vote_AccountId",
                table: "lecture_vote",
                newName: "IX_lecture_vote_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_lecture_vote",
                table: "lecture_vote",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_lecture_vote_account_AccountId",
                table: "lecture_vote",
                column: "AccountId",
                principalTable: "account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_lecture_vote_lecturer_LectureId",
                table: "lecture_vote",
                column: "LectureId",
                principalTable: "lecturer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
