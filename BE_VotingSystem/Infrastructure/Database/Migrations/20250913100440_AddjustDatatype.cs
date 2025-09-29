using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddjustDatatype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateIndex(
                name: "IX_lecture_vote_LectureId_AccountId_VotedAt",
                table: "lecture_vote",
                columns: new[] { "LectureId", "AccountId", "VotedAt" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_lecture_vote_LectureId_AccountId_VotedAt",
                table: "lecture_vote");

            migrationBuilder.AlterColumn<DateTime>(
                name: "VotedAt",
                table: "lecture_vote",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "NOW(6)",
                oldClrType: typeof(DateOnly),
                oldType: "DATE",
                oldDefaultValueSql: "CURDATE()");

            migrationBuilder.AddColumn<DateOnly>(
                name: "VotedDate",
                table: "lecture_vote",
                type: "date",
                nullable: false,
                computedColumnSql: "DATE(VotedAt)");

            migrationBuilder.CreateIndex(
                name: "IX_lecture_vote_LectureId_AccountId_VotedDate",
                table: "lecture_vote",
                columns: new[] { "LectureId", "AccountId", "VotedDate" },
                unique: true);
        }
    }
}
