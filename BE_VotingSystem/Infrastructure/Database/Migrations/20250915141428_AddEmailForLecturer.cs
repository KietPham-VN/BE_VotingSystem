using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailForLecturer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "lecture",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "lecture");
        }
    }
}
