using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameWebImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WebImage",
                table: "WebImage");

            migrationBuilder.RenameTable(
                name: "WebImage",
                newName: "web_image");

            migrationBuilder.AddPrimaryKey(
                name: "PK_web_image",
                table: "web_image",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_web_image",
                table: "web_image");

            migrationBuilder.RenameTable(
                name: "web_image",
                newName: "WebImage");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WebImage",
                table: "WebImage",
                column: "Name");
        }
    }
}
