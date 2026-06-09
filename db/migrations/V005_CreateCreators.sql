-- V005: Create Creators and CreatorRoles tables
CREATE TABLE IF NOT EXISTS "CreatorRoles" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS "Creators" (
    "Id" SERIAL PRIMARY KEY,
    "ComicVineId" INTEGER,
    "Name" VARCHAR(255) NOT NULL,
    "BirthDate" DATE,
    "Country" VARCHAR(100),
    "Description" TEXT,
    "ImageUrl" VARCHAR(500)
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Creators_ComicVineId" ON "Creators" ("ComicVineId") WHERE "ComicVineId" IS NOT NULL;
CREATE INDEX IF NOT EXISTS "IX_Creators_Name" ON "Creators" ("Name");