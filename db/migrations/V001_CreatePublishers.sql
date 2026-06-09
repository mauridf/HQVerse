-- V001: Create Publishers table
CREATE TABLE IF NOT EXISTS "Publishers" (
    "Id" SERIAL PRIMARY KEY,
    "ComicVineId" INTEGER,
    "Name" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "Country" VARCHAR(100),
    "FoundationDate" DATE,
    "Website" VARCHAR(500),
    "LogoUrl" VARCHAR(500),
    "BannerUrl" VARCHAR(500),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Publishers_ComicVineId" ON "Publishers" ("ComicVineId") WHERE "ComicVineId" IS NOT NULL;
CREATE INDEX IF NOT EXISTS "IX_Publishers_Name" ON "Publishers" ("Name");