-- V006: Create ComicSeries table
CREATE TABLE IF NOT EXISTS "ComicSeries" (
    "Id" SERIAL PRIMARY KEY,
    "ComicVineId" INTEGER,
    "Name" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "PublisherId" INTEGER REFERENCES "Publishers"("Id") ON DELETE SET NULL,
    "UniverseId" INTEGER REFERENCES "Universes"("Id") ON DELETE SET NULL,
    "StartYear" INTEGER,
    "EndYear" INTEGER,
    "TotalIssues" INTEGER,
    "ImageUrl" VARCHAR(500),
    "BannerUrl" VARCHAR(500)
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_ComicSeries_ComicVineId" ON "ComicSeries" ("ComicVineId") WHERE "ComicVineId" IS NOT NULL;
CREATE INDEX IF NOT EXISTS "IX_ComicSeries_Name" ON "ComicSeries" ("Name");
CREATE INDEX IF NOT EXISTS "IX_ComicSeries_PublisherId" ON "ComicSeries" ("PublisherId");