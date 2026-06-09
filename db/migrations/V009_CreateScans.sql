-- V009: Create Scans, ScanGroups, and ScanLinks tables
CREATE TABLE IF NOT EXISTS "ScanGroups" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "LogoUrl" VARCHAR(500),
    "Website" VARCHAR(500),
    "Discord" VARCHAR(255),
    "Telegram" VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS "Scans" (
    "Id" SERIAL PRIMARY KEY,
    "IssueId" INTEGER NOT NULL REFERENCES "ComicIssues"("Id") ON DELETE CASCADE,
    "ScanGroupId" INTEGER REFERENCES "ScanGroups"("Id") ON DELETE SET NULL,
    "Version" VARCHAR(50),
    "Language" VARCHAR(10) NOT NULL DEFAULT 'pt-BR',
    "Pages" INTEGER,
    "FileSize" BIGINT,
    "Format" VARCHAR(10),
    "Quality" VARCHAR(50),
    "UploaderUserId" INTEGER,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS "ScanLinks" (
    "Id" SERIAL PRIMARY KEY,
    "ScanId" INTEGER NOT NULL REFERENCES "Scans"("Id") ON DELETE CASCADE,
    "Type" VARCHAR(50) NOT NULL,
    "Url" VARCHAR(1000) NOT NULL,
    "IsOnline" BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE INDEX IF NOT EXISTS "IX_Scans_IssueId" ON "Scans" ("IssueId");
CREATE INDEX IF NOT EXISTS "IX_Scans_ScanGroupId" ON "Scans" ("ScanGroupId");
CREATE INDEX IF NOT EXISTS "IX_ScanLinks_ScanId" ON "ScanLinks" ("ScanId");