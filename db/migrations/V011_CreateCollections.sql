-- V011: Create UserCollections and CollectionIssues
CREATE TABLE IF NOT EXISTS "UserCollections" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "Name" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "IsPublic" BOOLEAN NOT NULL DEFAULT FALSE,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS "CollectionIssues" (
    "CollectionId" INTEGER NOT NULL REFERENCES "UserCollections"("Id") ON DELETE CASCADE,
    "IssueId" INTEGER NOT NULL REFERENCES "ComicIssues"("Id") ON DELETE CASCADE,
    "ReadStatus" VARCHAR(50) NOT NULL DEFAULT 'WISHLIST',
    "Rating" INTEGER CHECK ("Rating" >= 1 AND "Rating" <= 10),
    "Favorite" BOOLEAN NOT NULL DEFAULT FALSE,
    "Notes" TEXT,
    "AddedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    PRIMARY KEY ("CollectionId", "IssueId")
);

CREATE INDEX IF NOT EXISTS "IX_UserCollections_UserId" ON "UserCollections" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_CollectionIssues_CollectionId" ON "CollectionIssues" ("CollectionId");
CREATE INDEX IF NOT EXISTS "IX_CollectionIssues_IssueId" ON "CollectionIssues" ("IssueId");