using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLectureConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "lecturer",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_lecturer_AccountName",
                table: "lecturer",
                column: "AccountName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_lecturer_AccountName",
                table: "lecturer");

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "lecturer");
        }
    }
}
