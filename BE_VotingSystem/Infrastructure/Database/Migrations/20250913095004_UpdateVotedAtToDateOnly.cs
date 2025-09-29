using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVotedAtToDateOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop foreign key constraints first
            migrationBuilder.Sql("ALTER TABLE lecture_vote DROP FOREIGN KEY FK_lecture_vote_Lecture_LectureId;");
            migrationBuilder.Sql("ALTER TABLE lecture_vote DROP FOREIGN KEY FK_lecture_vote_Account_AccountId;");
            
            // Step 2: Drop the unique index
            migrationBuilder.DropIndex(
                name: "IX_lecture_vote_LectureId_AccountId_VotedDate",
                table: "lecture_vote");

            // Step 3: Drop the computed column
            migrationBuilder.DropColumn(
                name: "VotedDate",
                table: "lecture_vote");

            // Step 4: Alter the VotedAt column to DATE type using raw SQL
            migrationBuilder.Sql("ALTER TABLE lecture_vote MODIFY COLUMN VotedAt DATE NOT NULL DEFAULT (CURDATE());");

            // Step 5: Create the new unique index
            migrationBuilder.CreateIndex(
                name: "IX_lecture_vote_LectureId_AccountId_VotedAt",
                table: "lecture_vote",
                columns: new[] { "LectureId", "AccountId", "VotedAt" },
                unique: true);

            // Step 6: Recreate foreign key constraints
            migrationBuilder.Sql("ALTER TABLE lecture_vote ADD CONSTRAINT FK_lecture_vote_Lecture_LectureId FOREIGN KEY (LectureId) REFERENCES lecture (Id) ON DELETE CASCADE;");
            migrationBuilder.Sql("ALTER TABLE lecture_vote ADD CONSTRAINT FK_lecture_vote_Account_AccountId FOREIGN KEY (AccountId) REFERENCES account (Id) ON DELETE CASCADE;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop foreign key constraints first
            migrationBuilder.Sql("ALTER TABLE lecture_vote DROP FOREIGN KEY FK_lecture_vote_Lecture_LectureId;");
            migrationBuilder.Sql("ALTER TABLE lecture_vote DROP FOREIGN KEY FK_lecture_vote_Account_AccountId;");
            
            // Step 2: Drop the current unique index
            migrationBuilder.DropIndex(
                name: "IX_lecture_vote_LectureId_AccountId_VotedAt",
                table: "lecture_vote");

            // Step 3: Alter VotedAt back to DateTime using raw SQL
            migrationBuilder.Sql("ALTER TABLE lecture_vote MODIFY COLUMN VotedAt DATETIME(6) NOT NULL DEFAULT (NOW(6));");

            // Step 4: Add back the computed column
            migrationBuilder.AddColumn<DateOnly>(
                name: "VotedDate",
                table: "lecture_vote",
                type: "date",
                nullable: false,
                computedColumnSql: "DATE(VotedAt)");

            // Step 5: Create the original unique index
            migrationBuilder.CreateIndex(
                name: "IX_lecture_vote_LectureId_AccountId_VotedDate",
                table: "lecture_vote",
                columns: new[] { "LectureId", "AccountId", "VotedDate" },
                unique: true);

            // Step 6: Recreate foreign key constraints
            migrationBuilder.Sql("ALTER TABLE lecture_vote ADD CONSTRAINT FK_lecture_vote_Lecture_LectureId FOREIGN KEY (LectureId) REFERENCES lecture (Id) ON DELETE CASCADE;");
            migrationBuilder.Sql("ALTER TABLE lecture_vote ADD CONSTRAINT FK_lecture_vote_Account_AccountId FOREIGN KEY (AccountId) REFERENCES account (Id) ON DELETE CASCADE;");
        }
    }
}
