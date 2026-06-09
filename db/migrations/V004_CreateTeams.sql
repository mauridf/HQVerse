-- V004: Create Teams table
CREATE TABLE IF NOT EXISTS "Teams" (
    "Id" SERIAL PRIMARY KEY,
    "ComicVineId" INTEGER,
    "Name" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "PublisherId" INTEGER REFERENCES "Publishers"("Id") ON DELETE SET NULL,
    "UniverseId" INTEGER REFERENCES "Universes"("Id") ON DELETE SET NULL,
    "ImageUrl" VARCHAR(500)
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Teams_ComicVineId" ON "Teams" ("ComicVineId") WHERE "ComicVineId" IS NOT NULL;
CREATE INDEX IF NOT EXISTS "IX_Teams_Name" ON "Teams" ("Name");