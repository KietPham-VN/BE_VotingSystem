using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class refreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "refresh_token",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Token = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Expires = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_token", x => x.Id);
                    table.CheckConstraint("CK_RefreshToken_Expires_Future", "`Expires` IS NOT NULL");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_Expires",
                table: "refresh_token",
                column: "Expires");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_Token",
                table: "refresh_token",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "refresh_token");
        }
    }
}