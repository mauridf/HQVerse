-- V010: Create Users table
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Username" VARCHAR(100) NOT NULL UNIQUE,
    "DisplayName" VARCHAR(255) NOT NULL,
    "Email" VARCHAR(255) NOT NULL UNIQUE,
    "PasswordHash" VARCHAR(500) NOT NULL,
    "AvatarUrl" VARCHAR(500),
    "BannerUrl" VARCHAR(500),
    "Bio" TEXT,
    "Role" VARCHAR(50) NOT NULL DEFAULT 'User',
    "RefreshToken" VARCHAR(500),
    "RefreshTokenExpiryTime" TIMESTAMP,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users" ("Email");
CREATE INDEX IF NOT EXISTS "IX_Users_Username" ON "Users" ("Username");