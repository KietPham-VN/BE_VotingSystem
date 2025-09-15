using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Account_Semester_Range",
                table: "account");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "account",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Account_Semester_Range",
                table: "account",
                sql: "(Semester IS NULL OR (Semester >= 0 AND Semester <= 9))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Account_Semester_Range",
                table: "account");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "account");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Account_Semester_Range",
                table: "account",
                sql: "(Semester IS NULL OR (Semester >= 1 AND Semester <= 9))");
        }
    }
}