-- ============================================
-- CRÉER LES RÔLES SSO STANDARDS
-- Architecture simplifiée basée sur rôles
-- ============================================

USE ONEE_SSO;
GO

PRINT '========================================';
PRINT 'CRÉATION DES RÔLES SSO STANDARDS';
PRINT '========================================';

-- Désactiver temporairement les contraintes FK
ALTER TABLE UserRoles NOCHECK CONSTRAINT ALL;
ALTER TABLE RolePermissions NOCHECK CONSTRAINT ALL;

-- Supprimer les anciens rôles (prudence!)
-- DELETE FROM RolePermissions;
-- DELETE FROM UserRoles;
-- DELETE FROM Roles;

-- 1. RÔLES ADMIN SSO
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'SuperAdmin')
    INSERT INTO Roles (Id, Name, Description, CreatedAt)
    VALUES (NEWID(), 'SuperAdmin', '👑 Super Administrateur SSO - Accès complet système', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Admin')
    INSERT INTO Roles (Id, Name, Description, CreatedAt)
    VALUES (NEWID(), 'Admin', '🔐 Administrateur SSO - Gestion utilisateurs et rôles', GETUTCDATE());

-- 2. RÔLES RH
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'AdminRH')
    INSERT INTO Roles (Id, Name, Description, CreatedAt)
    VALUES (NEWID(), 'AdminRH', '🏢 Administrateur RH - Gestion complète personnel', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'ChefRH')
    INSERT INTO Roles (Id, Name, Description, CreatedAt)
    VALUES (NEWID(), 'ChefRH', '👔 Chef Service RH - Gestion RH', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Directeur')
    INSERT INTO Roles (Id, Name, Description, CreatedAt)
    VALUES (NEWID(), 'Directeur', '🏢 Directeur - Gestion d''une direction', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'ChefDeService')
    INSERT INTO Roles (Id, Name, Description, CreatedAt)
    VALUES (NEWID(), 'ChefDeService', '👔 Chef de Service - Gestion d''un service', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Employe')
    INSERT INTO Roles (Id, Name, Description, CreatedAt)
    VALUES (NEWID(), 'Employe', '👤 Employé - Consultation données personnelles', GETUTCDATE());

-- 3. RÔLES TIMS (Interventions Techniques)
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'DirecteurTechnique')
    INSERT INTO Roles (Id, Name, Description, CreatedAt)
    VALUES (NEWID(), 'DirecteurTechnique', '🔧 Directeur Technique - Supervision interventions', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'ChefEquipe')
    INSERT INTO Roles (Id, Name, Description, CreatedAt)
    VALUES (NEWID(), 'ChefEquipe', '👷 Chef d''Équipe Technique - Gestion équipe interventions', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Technicien')
    INSERT INTO Roles (Id, Name, Description, CreatedAt)
    VALUES (NEWID(), 'Technicien', '🔧 Technicien - Création et suivi interventions', GETUTCDATE());

IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Operateur')
    INSERT INTO Roles (Id, Name, Description, CreatedAt)
    VALUES (NEWID(), 'Operateur', '⚙️ Opérateur - Consultation interventions', GETUTCDATE());

-- 4. RÔLES EAMS (Gestion Patrimoine)
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'AdminPatrimoine')
    INSERT INTO Roles (Id, Name, Description, CreatedAt)
    VALUES (NEWID(), 'AdminPatrimoine', '⚙️ Administrateur Patrimoine - Gestion complète équipements', GETUTCDATE());

-- Réactiver les contraintes FK
ALTER TABLE UserRoles CHECK CONSTRAINT ALL;
ALTER TABLE RolePermissions CHECK CONSTRAINT ALL;

PRINT '';
PRINT '========================================';
PRINT 'RÔLES CRÉÉS';
PRINT '========================================';

SELECT 
    Name AS RoleName,
    Description,
    CASE 
        WHEN Name IN ('SuperAdmin', 'Admin') THEN '👥 Admin SSO'
        WHEN Name IN ('AdminRH', 'ChefRH', 'Directeur', 'ChefDeService', 'Employe') THEN '🏢 RH'
        WHEN Name IN ('DirecteurTechnique', 'ChefEquipe', 'Technicien', 'Operateur') THEN '🔧 TIMS'
        WHEN Name IN ('AdminPatrimoine') THEN '⚙️ EAMS'
        ELSE '❓ Autre'
    END AS Application
FROM Roles
ORDER BY 
    CASE 
        WHEN Name IN ('SuperAdmin', 'Admin') THEN 1
        WHEN Name IN ('AdminRH', 'ChefRH', 'Directeur', 'ChefDeService', 'Employe') THEN 2
        WHEN Name IN ('DirecteurTechnique', 'ChefEquipe', 'Technicien', 'Operateur') THEN 3
        WHEN Name IN ('AdminPatrimoine') THEN 4
        ELSE 5
    END,
    Name;

PRINT '';
PRINT '========================================';
PRINT 'RECOMMANDATIONS';
PRINT '========================================';
PRINT '✅ Rôles standards créés';
PRINT '📝 Mapping documenté dans MAPPING_ROLES_SSO_APPS.md';
PRINT '🎯 Chaque app mappe ces rôles SSO vers ses rôles locaux';
PRINT '';
PRINT '💡 EXEMPLES D''ATTRIBUTION :';
PRINT '  - Chef RH → AdminRH (accès app RH uniquement)';
PRINT '  - Directeur Tech → DirecteurTechnique (accès TIMS + RH lecture)';
PRINT '  - Admin Patrimoine → AdminPatrimoine (accès EAMS complet)';
PRINT '  - Tech multi-apps → Technicien (accès TIMS + EAMS + RH lecture)';
