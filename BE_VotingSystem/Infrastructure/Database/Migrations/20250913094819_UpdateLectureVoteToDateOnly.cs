using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLectureVoteToDateOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Create a temporary table with the new structure
            migrationBuilder.Sql(@"
                CREATE TABLE lecture_vote_temp (
                    Id CHAR(36) NOT NULL,
                    LectureId CHAR(36) NOT NULL,
                    AccountId CHAR(36) NOT NULL,
                    VotedAt DATE NOT NULL DEFAULT (CURDATE()),
                    PRIMARY KEY (Id),
                    INDEX IX_lecture_vote_temp_LectureId_AccountId_VotedAt (LectureId, AccountId, VotedAt),
                    UNIQUE INDEX IX_lecture_vote_temp_LectureId_AccountId_VotedAt_Unique (LectureId, AccountId, VotedAt),
                    CONSTRAINT FK_lecture_vote_temp_Lecture_LectureId 
                        FOREIGN KEY (LectureId) REFERENCES lecture (Id) ON DELETE CASCADE,
                    CONSTRAINT FK_lecture_vote_temp_Account_AccountId 
                        FOREIGN KEY (AccountId) REFERENCES account (Id) ON DELETE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
            ");

            // Step 2: Copy data from old table to new table
            migrationBuilder.Sql(@"
                INSERT INTO lecture_vote_temp (Id, LectureId, AccountId, VotedAt)
                SELECT Id, LectureId, AccountId, DATE(VotedAt) as VotedAt
                FROM lecture_vote;
            ");

            // Step 3: Drop the old table
            migrationBuilder.Sql("DROP TABLE lecture_vote;");

            // Step 4: Rename the new table
            migrationBuilder.Sql("RENAME TABLE lecture_vote_temp TO lecture_vote;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Create a temporary table with the old structure
            migrationBuilder.Sql(@"
                CREATE TABLE lecture_vote_temp (
                    Id CHAR(36) NOT NULL,
                    LectureId CHAR(36) NOT NULL,
                    AccountId CHAR(36) NOT NULL,
                    VotedAt DATETIME(6) NOT NULL DEFAULT (NOW(6)),
                    VotedDate DATE NOT NULL GENERATED ALWAYS AS (DATE(VotedAt)) STORED,
                    PRIMARY KEY (Id),
                    INDEX IX_lecture_vote_temp_LectureId_AccountId_VotedDate (LectureId, AccountId, VotedDate),
                    UNIQUE INDEX IX_lecture_vote_temp_LectureId_AccountId_VotedDate_Unique (LectureId, AccountId, VotedDate),
                    CONSTRAINT FK_lecture_vote_temp_Lecture_LectureId 
                        FOREIGN KEY (LectureId) REFERENCES lecture (Id) ON DELETE CASCADE,
                    CONSTRAINT FK_lecture_vote_temp_Account_AccountId 
                        FOREIGN KEY (AccountId) REFERENCES account (Id) ON DELETE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
            ");

            // Step 2: Copy data from new table to old table
            migrationBuilder.Sql(@"
                INSERT INTO lecture_vote_temp (Id, LectureId, AccountId, VotedAt)
                SELECT Id, LectureId, AccountId, CAST(VotedAt AS DATETIME) as VotedAt
                FROM lecture_vote;
            ");

            // Step 3: Drop the new table
            migrationBuilder.Sql("DROP TABLE lecture_vote;");

            // Step 4: Rename the old table back
            migrationBuilder.Sql("RENAME TABLE lecture_vote_temp TO lecture_vote;");
        }
    }
}
