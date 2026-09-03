# 📜 Scripts

Ce dossier contient des scripts utiles pour la configuration et la maintenance du projet.

## 📁 Structure

```
scripts/
├── database/          # Scripts SQL pour initialisation/configuration
│   ├── CREATE_NOTIFICATIONS_TABLE.sql
│   ├── CREATE_ROLES_SSO_STANDARDS.sql
│   └── CREATE_USER_CONSENTS_TABLE.sql
└── README.md
```

## 🗄️ Scripts de base de données (`database/`)

### `CREATE_NOTIFICATIONS_TABLE.sql`
Crée la table `Notifications` dans la base de données SSO pour le système de notifications par email (réinitialisation de mot de passe, etc.).

### `CREATE_ROLES_SSO_STANDARDS.sql`
Crée les rôles standards du système (SuperAdmin, AdministrateurRH, UtilisateurRH, etc.).

### `CREATE_USER_CONSENTS_TABLE.sql`
Crée la table `UserConsents` pour stocker les consentements OIDC des utilisateurs (évite l'affichage répété de la page de consentement).

## 🚀 Utilisation

Ces scripts sont exécutés automatiquement par le système lors du premier démarrage via Entity Framework Core Migrations et Seeders.

Pour exécuter manuellement un script :

```bash
sqlcmd -S YOUR_SERVER\SQLEXPRESS -d ONEE_SSO -i scripts/database/SCRIPT_NAME.sql
```

Ou depuis SQL Server Management Studio (SSMS) :
1. Ouvrir SSMS
2. Se connecter à votre instance SQL Server
3. Sélectionner la base de données `ONEE_SSO`
4. Ouvrir le script
5. Exécuter (F5)
