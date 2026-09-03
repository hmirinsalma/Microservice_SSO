-- =============================================
-- Script: Créer la table UserConsents pour mémoriser les consentements utilisateurs
-- Description: Permet de skip la page d'autorisation après la première connexion
-- Date: 2026-08-30
-- =============================================

USE ONEE_SSO;
GO

-- Vérifier si la table existe déjà
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserConsents')
BEGIN
    CREATE TABLE [dbo].[UserConsents] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [ClientId] NVARCHAR(100) NOT NULL,
        [Scopes] NVARCHAR(500) NOT NULL,
        [GrantedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ExpiresAt] DATETIME2 NULL,
        [IpAddress] NVARCHAR(50) NULL,
        
        CONSTRAINT [PK_UserConsents] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserConsents_Users] FOREIGN KEY ([UserId]) 
            REFERENCES [Users]([Id]) ON DELETE CASCADE,
        CONSTRAINT [UQ_UserConsents_UserId_ClientId] UNIQUE ([UserId], [ClientId])
    );

    -- Index pour recherche rapide
    CREATE INDEX [IX_UserConsents_UserId] ON [UserConsents] ([UserId]);
    CREATE INDEX [IX_UserConsents_ClientId] ON [UserConsents] ([ClientId]);

    PRINT '✅ Table UserConsents créée avec succès';
END
ELSE
BEGIN
    PRINT '⚠️ Table UserConsents existe déjà';
END
GO
