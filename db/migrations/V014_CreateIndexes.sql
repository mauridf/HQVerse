-- V014: Performance indexes for common queries
CREATE INDEX IF NOT EXISTS "IX_ComicIssues_CoverDate" ON "ComicIssues" ("CoverDate");
CREATE INDEX IF NOT EXISTS "IX_ComicIssues_ReleaseDate" ON "ComicIssues" ("ReleaseDate");
CREATE INDEX IF NOT EXISTS "IX_ComicIssues_CreatedAt" ON "ComicIssues" ("CreatedAt");

CREATE INDEX IF NOT EXISTS "IX_Scans_Language" ON "Scans" ("Language");
CREATE INDEX IF NOT EXISTS "IX_Scans_CreatedAt" ON "Scans" ("CreatedAt");

CREATE INDEX IF NOT EXISTS "IX_Reviews_CreatedAt" ON "Reviews" ("CreatedAt");
CREATE INDEX IF NOT EXISTS "IX_Reviews_Rating" ON "Reviews" ("Rating");

-- Full text search indexes for PostgreSQL
CREATE INDEX IF NOT EXISTS "IX_Characters_Name_FTS" ON "Characters" USING gin (to_tsvector('portuguese', "Name"));
CREATE INDEX IF NOT EXISTS "IX_ComicSeries_Name_FTS" ON "ComicSeries" USING gin (to_tsvector('portuguese', "Name"));
CREATE INDEX IF NOT EXISTS "IX_Publishers_Name_FTS" ON "Publishers" USING gin (to_tsvector('portuguese', "Name"));