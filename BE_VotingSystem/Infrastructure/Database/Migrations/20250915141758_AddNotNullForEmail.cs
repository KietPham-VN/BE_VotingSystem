using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddNotNullForEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lecture_vote_lecture_LectureId",
                table: "lecture_vote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_lecture",
                table: "lecture");

            migrationBuilder.RenameTable(
                name: "lecture",
                newName: "lecturer");

            migrationBuilder.RenameIndex(
                name: "IX_lecture_Name",
                table: "lecturer",
                newName: "IX_lecturer_Name");

            migrationBuilder.RenameIndex(
                name: "IX_lecture_Department",
                table: "lecturer",
                newName: "IX_lecturer_Department");

            migrationBuilder.UpdateData(
                table: "lecturer",
                keyColumn: "Email",
                keyValue: null,
                column: "Email",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "lecturer",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_lecturer",
                table: "lecturer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_lecture_vote_lecturer_LectureId",
                table: "lecture_vote",
                column: "LectureId",
                principalTable: "lecturer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lecture_vote_lecturer_LectureId",
                table: "lecture_vote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_lecturer",
                table: "lecturer");

            migrationBuilder.RenameTable(
                name: "lecturer",
                newName: "lecture");

            migrationBuilder.RenameIndex(
                name: "IX_lecturer_Name",
                table: "lecture",
                newName: "IX_lecture_Name");

            migrationBuilder.RenameIndex(
                name: "IX_lecturer_Department",
                table: "lecture",
                newName: "IX_lecture_Department");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "lecture",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_lecture",
                table: "lecture",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_lecture_vote_lecture_LectureId",
                table: "lecture_vote",
                column: "LectureId",
                principalTable: "lecture",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
