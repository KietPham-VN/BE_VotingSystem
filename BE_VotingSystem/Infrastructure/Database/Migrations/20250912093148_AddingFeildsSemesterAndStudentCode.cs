using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddingFeildsSemesterAndStudentCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET @col_exists := (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'account' AND COLUMN_NAME = 'Semester'
);
SET @sql := IF(@col_exists = 0,
    'ALTER TABLE `account` ADD COLUMN `Semester` tinyint unsigned NULL',
    'SELECT 1'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
", suppressTransaction: false);

            // Ensure Semester is nullable even if it already existed as NOT NULL
            migrationBuilder.Sql(@"
SET @is_not_nullable := (
    SELECT CASE WHEN IS_NULLABLE = 'NO' THEN 1 ELSE 0 END
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'account' AND COLUMN_NAME = 'Semester'
);
SET @sql := IF(@is_not_nullable = 1,
    'ALTER TABLE `account` MODIFY COLUMN `Semester` tinyint unsigned NULL',
    'SELECT 1'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
", suppressTransaction: false);

            migrationBuilder.Sql(@"
SET @col_exists := (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'account' AND COLUMN_NAME = 'StudentCode'
);
SET @sql := IF(@col_exists = 0,
    'ALTER TABLE `account` ADD COLUMN `StudentCode` varchar(8) NULL',
    'SELECT 1'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
", suppressTransaction: false);

            // Ensure StudentCode is nullable even if it already existed as NOT NULL
            migrationBuilder.Sql(@"
SET @is_not_nullable := (
    SELECT CASE WHEN IS_NULLABLE = 'NO' THEN 1 ELSE 0 END
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'account' AND COLUMN_NAME = 'StudentCode'
);
SET @sql := IF(@is_not_nullable = 1,
    'ALTER TABLE `account` MODIFY COLUMN `StudentCode` varchar(8) NULL',
    'SELECT 1'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
", suppressTransaction: false);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Account_Semester_Range",
                table: "account",
                sql: "(Semester IS NULL OR (Semester >= 1 AND Semester <= 9))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Account_StudentCode_Format",
                table: "account",
                sql: "(StudentCode IS NULL OR UPPER(StudentCode) REGEXP '^(SS|SA|SE|CS|CA|CE|HS|HE|HA|QS|QA|QE|DS|DA|DE)[0-9]{6}$')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Account_Semester_Range",
                table: "account");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Account_StudentCode_Format",
                table: "account");

            migrationBuilder.Sql(@"
SET @col_exists := (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'account' AND COLUMN_NAME = 'Semester'
);
SET @sql := IF(@col_exists = 1,
    'ALTER TABLE `account` DROP COLUMN `Semester`',
    'SELECT 1'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
", suppressTransaction: false);

            migrationBuilder.Sql(@"
SET @col_exists := (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'account' AND COLUMN_NAME = 'StudentCode'
);
SET @sql := IF(@col_exists = 1,
    'ALTER TABLE `account` DROP COLUMN `StudentCode`',
    'SELECT 1'
);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
", suppressTransaction: false);
        }
    }
}