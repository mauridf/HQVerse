-- V002: Create Universes table
CREATE TABLE IF NOT EXISTS "Universes" (
    "Id" SERIAL PRIMARY KEY,
    "ComicVineId" INTEGER,
    "Name" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "LogoUrl" VARCHAR(500),
    "BannerUrl" VARCHAR(500)
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Universes_ComicVineId" ON "Universes" ("ComicVineId") WHERE "ComicVineId" IS NOT NULL;