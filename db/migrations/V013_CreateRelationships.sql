-- V013: Create relationship tables
CREATE TABLE IF NOT EXISTS "CharacterTeams" (
    "CharacterId" INTEGER NOT NULL REFERENCES "Characters"("Id") ON DELETE CASCADE,
    "TeamId" INTEGER NOT NULL REFERENCES "Teams"("Id") ON DELETE CASCADE,
    PRIMARY KEY ("CharacterId", "TeamId")
);

CREATE TABLE IF NOT EXISTS "IssueCharacters" (
    "IssueId" INTEGER NOT NULL REFERENCES "ComicIssues"("Id") ON DELETE CASCADE,
    "CharacterId" INTEGER NOT NULL REFERENCES "Characters"("Id") ON DELETE CASCADE,
    PRIMARY KEY ("IssueId", "CharacterId")
);

CREATE TABLE IF NOT EXISTS "IssueTeams" (
    "IssueId" INTEGER NOT NULL REFERENCES "ComicIssues"("Id") ON DELETE CASCADE,
    "TeamId" INTEGER NOT NULL REFERENCES "Teams"("Id") ON DELETE CASCADE,
    PRIMARY KEY ("IssueId", "TeamId")
);

CREATE TABLE IF NOT EXISTS "IssueCreators" (
    "IssueId" INTEGER NOT NULL REFERENCES "ComicIssues"("Id") ON DELETE CASCADE,
    "CreatorId" INTEGER NOT NULL REFERENCES "Creators"("Id") ON DELETE CASCADE,
    "CreatorRoleId" INTEGER NOT NULL REFERENCES "CreatorRoles"("Id") ON DELETE RESTRICT,
    PRIMARY KEY ("IssueId", "CreatorId", "CreatorRoleId")
);

CREATE TABLE IF NOT EXISTS "ReadingProgress" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "IssueId" INTEGER NOT NULL REFERENCES "ComicIssues"("Id") ON DELETE CASCADE,
    "CurrentPage" INTEGER NOT NULL DEFAULT 0,
    "ProgressPercent" DECIMAL(5,2) NOT NULL DEFAULT 0,
    "StartedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "FinishedAt" TIMESTAMP,
    UNIQUE ("UserId", "IssueId")
);

CREATE TABLE IF NOT EXISTS "UserFavorites" (
    "UserId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "EntityType" VARCHAR(50) NOT NULL,
    "EntityId" INTEGER NOT NULL,
    PRIMARY KEY ("UserId", "EntityType", "EntityId")
);

CREATE INDEX IF NOT EXISTS "IX_ReadingProgress_UserId" ON "ReadingProgress" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_UserFavorites_UserId" ON "UserFavorites" ("UserId");