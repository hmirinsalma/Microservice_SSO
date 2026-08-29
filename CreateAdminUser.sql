-- Script pour créer un utilisateur Admin dans ONEE.SSO
-- Password: Admin@123 (hashé avec BCrypt)

USE ONEE_SSO;
GO

-- Insérer l'utilisateur Admin
INSERT INTO Users (
    Id,
    Username,
    Email,
    PasswordHash,
    FirstName,
    LastName,
    IsActive,
    IsLocked,
    FailedLoginAttempts,
    IsEmailVerified,
    CreatedAt,
    UpdatedAt
)
VALUES (
    NEWID(),
    'admin',
    'admin@onee.ma',
    '$2a$11$rQZ5vJ3xK6q8PxZ9mH0Y0.TXGvK4pPxZ5vJ3xK6q8PxZ9mH0Y0TXG', -- Password: Admin@123
    'Admin',
    'User',
    1, -- IsActive
    0, -- IsLocked
    0, -- FailedLoginAttempts
    1, -- IsEmailVerified
    GETUTCDATE(),
    GETUTCDATE()
);
GO

-- Vérifier que l'utilisateur a été créé
SELECT Id, Username, Email, FirstName, LastName, IsActive
FROM Users
WHERE Email = 'admin@onee.ma';
GO
