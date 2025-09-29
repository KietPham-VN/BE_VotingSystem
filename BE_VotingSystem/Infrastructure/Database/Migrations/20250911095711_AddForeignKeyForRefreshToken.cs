using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyForRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "refresh_token",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_AccountId",
                table: "refresh_token",
                column: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_refresh_token_AccountId",
                table: "refresh_token");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "refresh_token");
        }
    }
}