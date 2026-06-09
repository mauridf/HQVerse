-- V012: Create Reviews, Comments, and ReviewLikes
CREATE TABLE IF NOT EXISTS "Reviews" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "IssueId" INTEGER NOT NULL REFERENCES "ComicIssues"("Id") ON DELETE CASCADE,
    "Title" VARCHAR(255) NOT NULL,
    "Content" TEXT,
    "Rating" INTEGER NOT NULL CHECK ("Rating" >= 1 AND "Rating" <= 10),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS "Comments" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "ReviewId" INTEGER NOT NULL REFERENCES "Reviews"("Id") ON DELETE CASCADE,
    "Content" TEXT NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS "ReviewLikes" (
    "UserId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "ReviewId" INTEGER NOT NULL REFERENCES "Reviews"("Id") ON DELETE CASCADE,
    PRIMARY KEY ("UserId", "ReviewId")
);

CREATE INDEX IF NOT EXISTS "IX_Reviews_UserId" ON "Reviews" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_Reviews_IssueId" ON "Reviews" ("IssueId");
CREATE INDEX IF NOT EXISTS "IX_Comments_ReviewId" ON "Comments" ("ReviewId");