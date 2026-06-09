-- V016: Seed initial data for CreatorRoles and other reference data

-- Creator Roles
INSERT INTO "CreatorRoles" ("Name") VALUES
    ('Writer'),
    ('Artist'),
    ('Penciler'),
    ('Colorist'),
    ('Editor'),
    ('CoverArtist'),
    ('Letterer'),
    ('Inker')
ON CONFLICT ("Name") DO NOTHING;

-- Create a system user for automated imports
INSERT INTO "Users" ("Username", "DisplayName", "Email", "PasswordHash", "Role")
VALUES (
    'system',
    'HQVerse System',
    'system@hqverse.internal',
    'NOT_A_REAL_PASSWORD_HASH_SYSTEM_ACCOUNT',
    'Admin'
)
ON CONFLICT ("Username") DO NOTHING;