-- V015: Create ExternalSource and ExternalMapping for Comic Vine integration
CREATE TABLE IF NOT EXISTS "ExternalSources" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL UNIQUE,
    "BaseUrl" VARCHAR(500),
    "ApiKeyRequired" BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS "ExternalMappings" (
    "Id" SERIAL PRIMARY KEY,
    "SourceId" INTEGER NOT NULL REFERENCES "ExternalSources"("Id") ON DELETE CASCADE,
    "ExternalId" INTEGER NOT NULL,
    "EntityType" VARCHAR(50) NOT NULL,
    "InternalId" INTEGER NOT NULL,
    "LastSync" TIMESTAMP NOT NULL DEFAULT NOW(),
    UNIQUE ("SourceId", "ExternalId", "EntityType")
);

CREATE INDEX IF NOT EXISTS "IX_ExternalMappings_InternalId" ON "ExternalMappings" ("InternalId", "EntityType");
CREATE INDEX IF NOT EXISTS "IX_ExternalMappings_ExternalId" ON "ExternalMappings" ("ExternalId");

-- Insert Comic Vine as default external source
INSERT INTO "ExternalSources" ("Name", "BaseUrl", "ApiKeyRequired")
VALUES ('ComicVine', 'https://comicvine.gamespot.com/api', TRUE)
ON CONFLICT ("Name") DO NOTHING;