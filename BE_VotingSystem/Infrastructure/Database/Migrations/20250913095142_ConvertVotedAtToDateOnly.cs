using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE_VotingSystem.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ConvertVotedAtToDateOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Use raw SQL to handle the conversion safely
            migrationBuilder.Sql(@"
                -- Step 1: Drop foreign key constraints
                ALTER TABLE lecture_vote DROP FOREIGN KEY IF EXISTS FK_lecture_vote_Lecture_LectureId;
                ALTER TABLE lecture_vote DROP FOREIGN KEY IF EXISTS FK_lecture_vote_Account_AccountId;
                
                -- Step 2: Drop the unique index
                ALTER TABLE lecture_vote DROP INDEX IF EXISTS IX_lecture_vote_LectureId_AccountId_VotedDate;
                
                -- Step 3: Drop the computed column
                ALTER TABLE lecture_vote DROP COLUMN IF EXISTS VotedDate;
                
                -- Step 4: Convert VotedAt to DATE type
                ALTER TABLE lecture_vote MODIFY COLUMN VotedAt DATE NOT NULL DEFAULT (CURDATE());
                
                -- Step 5: Create new unique index
                CREATE UNIQUE INDEX IX_lecture_vote_LectureId_AccountId_VotedAt 
                ON lecture_vote (LectureId, AccountId, VotedAt);
                
                -- Step 6: Recreate foreign key constraints
                ALTER TABLE lecture_vote ADD CONSTRAINT FK_lecture_vote_Lecture_LectureId 
                    FOREIGN KEY (LectureId) REFERENCES lecture (Id) ON DELETE CASCADE;
                ALTER TABLE lecture_vote ADD CONSTRAINT FK_lecture_vote_Account_AccountId 
                    FOREIGN KEY (AccountId) REFERENCES account (Id) ON DELETE CASCADE;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Use raw SQL to revert the changes
            migrationBuilder.Sql(@"
                -- Step 1: Drop foreign key constraints
                ALTER TABLE lecture_vote DROP FOREIGN KEY IF EXISTS FK_lecture_vote_Lecture_LectureId;
                ALTER TABLE lecture_vote DROP FOREIGN KEY IF EXISTS FK_lecture_vote_Account_AccountId;
                
                -- Step 2: Drop the current unique index
                ALTER TABLE lecture_vote DROP INDEX IF EXISTS IX_lecture_vote_LectureId_AccountId_VotedAt;
                
                -- Step 3: Convert VotedAt back to DATETIME
                ALTER TABLE lecture_vote MODIFY COLUMN VotedAt DATETIME(6) NOT NULL DEFAULT (NOW(6));
                
                -- Step 4: Add back the computed column
                ALTER TABLE lecture_vote ADD COLUMN VotedDate DATE NOT NULL GENERATED ALWAYS AS (DATE(VotedAt)) STORED;
                
                -- Step 5: Create original unique index
                CREATE UNIQUE INDEX IX_lecture_vote_LectureId_AccountId_VotedDate 
                ON lecture_vote (LectureId, AccountId, VotedDate);
                
                -- Step 6: Recreate foreign key constraints
                ALTER TABLE lecture_vote ADD CONSTRAINT FK_lecture_vote_Lecture_LectureId 
                    FOREIGN KEY (LectureId) REFERENCES lecture (Id) ON DELETE CASCADE;
                ALTER TABLE lecture_vote ADD CONSTRAINT FK_lecture_vote_Account_AccountId 
                    FOREIGN KEY (AccountId) REFERENCES account (Id) ON DELETE CASCADE;
            ");
        }
    }
}
