-- V003: Create Characters table
CREATE TABLE IF NOT EXISTS "Characters" (
    "Id" SERIAL PRIMARY KEY,
    "ComicVineId" INTEGER,
    "Name" VARCHAR(255) NOT NULL,
    "RealName" VARCHAR(255),
    "Description" TEXT,
    "FirstAppearance" VARCHAR(255),
    "Gender" VARCHAR(50),
    "Alignment" VARCHAR(50),
    "PublisherId" INTEGER REFERENCES "Publishers"("Id") ON DELETE SET NULL,
    "UniverseId" INTEGER REFERENCES "Universes"("Id") ON DELETE SET NULL,
    "ImageUrl" VARCHAR(500),
    "ThumbnailUrl" VARCHAR(500),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Characters_ComicVineId" ON "Characters" ("ComicVineId") WHERE "ComicVineId" IS NOT NULL;
CREATE INDEX IF NOT EXISTS "IX_Characters_Name" ON "Characters" ("Name");
CREATE INDEX IF NOT EXISTS "IX_Characters_PublisherId" ON "Characters" ("PublisherId");
CREATE INDEX IF NOT EXISTS "IX_Characters_UniverseId" ON "Characters" ("UniverseId");