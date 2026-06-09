-- V007: Create ComicIssues table
CREATE TABLE IF NOT EXISTS "ComicIssues" (
    "Id" SERIAL PRIMARY KEY,
    "ComicVineId" INTEGER,
    "SeriesId" INTEGER NOT NULL REFERENCES "ComicSeries"("Id") ON DELETE CASCADE,
    "IssueNumber" VARCHAR(50) NOT NULL,
    "Title" VARCHAR(255),
    "Synopsis" TEXT,
    "CoverDate" DATE,
    "ReleaseDate" DATE,
    "PageCount" INTEGER,
    "ISBN" VARCHAR(50),
    "UPC" VARCHAR(50),
    "CoverUrl" VARCHAR(500),
    "ThumbnailUrl" VARCHAR(500),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_ComicIssues_ComicVineId" ON "ComicIssues" ("ComicVineId") WHERE "ComicVineId" IS NOT NULL;
CREATE INDEX IF NOT EXISTS "IX_ComicIssues_SeriesId" ON "ComicIssues" ("SeriesId");
CREATE INDEX IF NOT EXISTS "IX_ComicIssues_IssueNumber" ON "ComicIssues" ("IssueNumber");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_ComicIssues_Series_Issue" ON "ComicIssues" ("SeriesId", "IssueNumber");