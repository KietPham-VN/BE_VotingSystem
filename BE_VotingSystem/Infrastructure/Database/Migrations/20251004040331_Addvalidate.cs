using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Addvalidate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_lecturer_AccountName",
                table: "lecturer");

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "lecturer",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_lecturer_AccountName",
                table: "lecturer",
                column: "AccountName",
                unique: true,
                filter: "AccountName IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_lecturer_AccountName",
                table: "lecturer");

            migrationBuilder.UpdateData(
                table: "lecturer",
                keyColumn: "AccountName",
                keyValue: null,
                column: "AccountName",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
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

            migrationBuilder.CreateIndex(
                name: "IX_lecturer_AccountName",
                table: "lecturer",
                column: "AccountName",
                unique: true);
        }
    }
}
