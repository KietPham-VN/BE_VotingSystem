using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLectureAndLectureVoteTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lecture",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Department = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quote = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AvatarUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lecture", x => x.Id);
                    table.CheckConstraint("CK_Lecture_Department_NotEmpty", "CHAR_LENGTH(TRIM(Department)) > 0");
                    table.CheckConstraint("CK_Lecture_Name_NotEmpty", "CHAR_LENGTH(TRIM(Name)) > 0");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "lecture_vote",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LectureId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AccountId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    VotedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "NOW(6)"),
                    VotedDate = table.Column<DateOnly>(type: "date", nullable: false, computedColumnSql: "DATE(VotedAt)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lecture_vote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lecture_vote_account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_lecture_vote_lecture_LectureId",
                        column: x => x.LectureId,
                        principalTable: "lecture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_lecture_Department",
                table: "lecture",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_lecture_Name",
                table: "lecture",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_lecture_vote_AccountId",
                table: "lecture_vote",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_lecture_vote_LectureId_AccountId_VotedDate",
                table: "lecture_vote",
                columns: new[] { "LectureId", "AccountId", "VotedDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lecture_vote_VotedAt",
                table: "lecture_vote",
                column: "VotedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lecture_vote");

            migrationBuilder.DropTable(
                name: "lecture");
        }
    }
}
