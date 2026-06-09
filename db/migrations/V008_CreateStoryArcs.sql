-- V008: Create StoryArcs table
CREATE TABLE IF NOT EXISTS "StoryArcs" (
    "Id" SERIAL PRIMARY KEY,
    "ComicVineId" INTEGER,
    "Name" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "PublisherId" INTEGER REFERENCES "Publishers"("Id") ON DELETE SET NULL,
    "ImageUrl" VARCHAR(500)
);

CREATE TABLE IF NOT EXISTS "StoryArcIssues" (
    "StoryArcId" INTEGER NOT NULL REFERENCES "StoryArcs"("Id") ON DELETE CASCADE,
    "IssueId" INTEGER NOT NULL REFERENCES "ComicIssues"("Id") ON DELETE CASCADE,
    "OrderNumber" INTEGER NOT NULL,
    PRIMARY KEY ("StoryArcId", "IssueId")
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_StoryArcs_ComicVineId" ON "StoryArcs" ("ComicVineId") WHERE "ComicVineId" IS NOT NULL;
CREATE INDEX IF NOT EXISTS "IX_StoryArcs_Name" ON "StoryArcs" ("Name");