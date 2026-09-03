# 🔐 ONEE SSO - Système d'Authentification Unique

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18-61DAFB)](https://reactjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.0-3178C6)](https://www.typescriptlang.org/)

> **Projet de Fin d'Année - ONEE**  
> Système SSO complet avec authentification centralisée et 3 applications clientes intégrées

---

## 📋 Table des Matières

- [À Propos](#-à-propos)
- [Fonctionnalités](#-fonctionnalités)
- [Architecture](#-architecture)
- [Technologies](#-technologies)
- [Prérequis](#-prérequis)
- [Installation](#-installation)
- [Configuration](#-configuration)
- [Utilisation](#-utilisation)
- [Structure du Projet](#-structure-du-projet)
- [Sécurité](#-sécurité)
- [Contribution](#-contribution)
- [Licence](#-licence)
- [Auteur](#-auteur)

---

## 🎯 À Propos

Ce projet a été développé dans le cadre d'un stage de fin d'année à l'**Office National de l'Électricité et de l'Eau Potable (ONEE)**.

L'objectif principal est de concevoir et implémenter un **microservice d'authentification unique (SSO)** centralisé, conforme aux standards **OAuth 2.0** et **OpenID Connect (OIDC)**, permettant une authentification unifiée et une gestion des autorisations granulaire (RBAC) pour l'ensemble des applications internes de l'organisme.

### Problématique Résolue

Avant ce projet, chaque application gérait ses propres utilisateurs et authentifications de manière décentralisée, entraînant :
- Duplication des données utilisateurs
- Maintenance complexe et coûteuse
- Absence d'audit centralisé
- Expérience utilisateur fragmentée (plusieurs identifiants)

### Solution Apportée

Un système SSO moderne permettant :
- **Une seule authentification** pour accéder à toutes les applications
- **Gestion centralisée** des utilisateurs, rôles et permissions
- **Audit complet** de toutes les actions d'authentification
- **Scalabilité** pour intégrer facilement de nouvelles applications

---

## ✨ Fonctionnalités

### 🔐 Microservice SSO

#### Authentification & Autorisation
- ✅ **OAuth 2.0 / OpenID Connect** (Authorization Code Flow avec PKCE)
- ✅ **Gestion des utilisateurs** (CRUD complet avec validation)
- ✅ **Système de rôles et permissions** (RBAC granulaire)
- ✅ **JWT sécurisés** (génération, validation, signature HMAC-SHA256)
- ✅ **Refresh tokens** avec rotation automatique
- ✅ **Consentement utilisateur** persistant par application
- ✅ **Protection force brute** (verrouillage automatique après 5 tentatives)

#### Sécurité
- ✅ **Hashage BCrypt** des mots de passe (work factor 12)
- ✅ **Claims personnalisés** dans les tokens
- ✅ **CORS configuré** pour clients autorisés
- ✅ **Validation PKCE** obligatoire
- ✅ **Audit logs complets** (connexions, accès, modifications)

#### Fonctionnalités Avancées
- ✅ **Interface d'administration** complète
- ✅ **Dashboard analytique** (statistiques temps réel)
- ✅ **Gestion des applications clientes** (ajout, modification, désactivation)
- ✅ **Logs d'audit consultables** avec filtres avancés
- ✅ **Notifications temps réel** (nouveaux accès, modifications)

---

### 📱 Applications Clientes Intégrées

Le projet inclut **3 applications complètes** déjà intégrées au SSO :

#### 1. **Gestion du Personnel (RH)** 👥
- Module complet de gestion des employés
- Organigrammes et départements
- Gestion des absences et congés
- Rapports RH personnalisés
- **Port** : 5174

#### 2. **TIMS (Technical Intervention Management System)** 🔧
- Gestion des interventions techniques
- Planification et suivi des maintenances
- Gestion des équipes terrain
- Rapports d'intervention
- **Port** : 5175

#### 3. **EAMS (Equipment & Asset Management System)** 📦
- Inventaire complet des équipements
- Gestion des maintenances préventives/correctives
- Historique des interventions
- Catégorisation et recherche avancée
- Dashboard analytique
- **Port** : 5173

---

## 🏗️ Architecture

### Architecture Globale

```
┌─────────────────────────────────────────────────────────────┐
│                     UTILISATEUR FINAL                        │
└───────────────────────┬─────────────────────────────────────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
        ▼               ▼               ▼
┌───────────────┐ ┌───────────────┐ ┌───────────────┐
│   RH Client   │ │  TIMS Client  │ │  EAMS Client  │
│  React 18     │ │  React 18     │ │  React 18     │
│  Port: 5174   │ │  Port: 5175   │ │  Port: 5173   │
└───────┬───────┘ └───────┬───────┘ └───────┬───────┘
        │                 │                 │
        └─────────────────┼─────────────────┘
                          │
                          ▼
              ┌───────────────────────┐
              │    SSO MICROSERVICE   │
              │   ASP.NET Core 9.0    │
              │    Port: 5205         │
              └───────────┬───────────┘
                          │
                          ▼
              ┌───────────────────────┐
              │   SQL SERVER          │
              │   Database: ONEE_SSO  │
              └───────────────────────┘
```

### Architecture du Microservice SSO

Le microservice SSO suit une **architecture en couches (Clean Architecture)** :

```
┌─────────────────────────────────────────────────────────────┐
│                      API Layer (ONEE.SSO.API)                │
│  Controllers, Middleware, JWT Authentication                 │
└────────────────────────────┬────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────┐
│              Application Layer (ONEE.SSO.Application)        │
│  Services, DTOs, Business Logic, Validators                  │
└────────────────────────────┬────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────┐
│                 Domain Layer (ONEE.SSO.Domain)               │
│  Entities, Enums, Domain Models                              │
└────────────────────────────┬────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────┐
│           Infrastructure Layer (ONEE.SSO.Infrastructure)     │
│  DbContext, Repositories, Migrations, Seeders                │
└─────────────────────────────────────────────────────────────┘
```

### Flux d'Authentification OAuth 2.0 / OIDC

```
┌──────────┐                                              ┌──────────┐
│  Client  │                                              │   SSO    │
│   App    │                                              │  Server  │
└────┬─────┘                                              └────┬─────┘
     │                                                          │
     │  1. Demande d'autorisation avec PKCE                    │
     │─────────────────────────────────────────────────────────>│
     │                                                          │
     │  2. Redirection vers page de login                      │
     │<─────────────────────────────────────────────────────────│
     │                                                          │
     │  3. Saisie des identifiants                             │
     │─────────────────────────────────────────────────────────>│
     │                                                          │
     │  4. Demande de consentement (première fois)             │
     │<─────────────────────────────────────────────────────────│
     │                                                          │
     │  5. Acceptation du consentement                         │
     │─────────────────────────────────────────────────────────>│
     │                                                          │
     │  6. Redirection avec Authorization Code                 │
     │<─────────────────────────────────────────────────────────│
     │                                                          │
     │  7. Échange du code contre access_token + id_token      │
     │─────────────────────────────────────────────────────────>│
     │                                                          │
     │  8. Tokens JWT retournés                                │
     │<─────────────────────────────────────────────────────────│
     │                                                          │
     │  9. Appels API avec Bearer Token                        │
     │─────────────────────────────────────────────────────────>│
     │                                                          │
     │ 10. Réponses avec données autorisées                    │
     │<─────────────────────────────────────────────────────────│
```

---

## 🛠️ Technologies

### Backend (Microservice SSO)

| Technologie | Version | Usage |
|------------|---------|-------|
| **ASP.NET Core** | 9.0 | Framework web principal |
| **Entity Framework Core** | 9.0 | ORM pour SQL Server |
| **SQL Server** | 2019+ | Base de données |
| **BCrypt.Net** | 0.1.0 | Hashage des mots de passe |
| **System.IdentityModel.Tokens.Jwt** | 8.1.2 | Génération/validation JWT |
| **Swashbuckle** | 6.6.2 | Documentation API (Swagger) |

### Frontend (Applications Clientes)

| Technologie | Version | Usage |
|------------|---------|-------|
| **React** | 18.3.1 | Framework UI |
| **TypeScript** | 5.5.3 | Typage statique |
| **Vite** | 5.3.4 | Build tool moderne |
| **React Router** | 6.26.0 | Routing |
| **Axios** | 1.7.3 | Client HTTP |
| **Tailwind CSS** | 3.4.7 | Framework CSS |
| **Recharts** | 2.12.7 | Graphiques/charts |
| **Lucide React** | 0.424.0 | Icônes |

### Outils de Développement

- **PowerShell** - Scripts d'automatisation
- **Git** - Contrôle de version
- **Visual Studio 2022 / VS Code** - IDEs

---

## ✅ Prérequis

Avant de commencer, assurez-vous d'avoir installé :

### Obligatoire

- [x] **Windows 10/11** (pour les scripts PowerShell)
- [x] **.NET 9.0 SDK** - [Télécharger](https://dotnet.microsoft.com/download/dotnet/9.0)
- [x] **Node.js v18+** - [Télécharger](https://nodejs.org/)
- [x] **SQL Server 2019+** (Express ou Developer Edition)
  - Instance : `SQLEXPRESS` ou configuration personnalisée
  - Windows Authentication activée

### Optionnel

- [ ] **Visual Studio 2022** - Pour le développement .NET
- [ ] **VS Code** - Pour le développement frontend
- [ ] **SQL Server Management Studio (SSMS)** - Pour gérer la BDD

### Vérification des prérequis

```powershell
# Vérifier .NET
dotnet --version
# Attendu: 9.0.x

# Vérifier Node.js
node --version
# Attendu: v18.x.x ou supérieur

# Vérifier npm
npm --version
# Attendu: 9.x.x ou supérieur

# Vérifier SQL Server
sqlcmd -S localhost\SQLEXPRESS -Q "SELECT @@VERSION"
# Doit retourner la version de SQL Server
```

---

## 🚀 Installation

### Option 1 : Installation Automatique (Recommandée)

Un script PowerShell automatise l'installation complète :

```powershell
# Cloner le repository
git clone https://github.com/VOTRE_USERNAME/ONEE.SSO.git
cd ONEE.SSO

# Lancer l'installation complète
.\SETUP_COMPLET.ps1
```

**Ce script va automatiquement :**
1. ✅ Vérifier les prérequis (.NET, Node.js, SQL Server)
2. ✅ Créer la base de données `ONEE_SSO`
3. ✅ Appliquer les migrations Entity Framework
4. ✅ Insérer les données initiales (utilisateurs, rôles, applications)
5. ✅ Installer les dépendances npm des 3 clients
6. ✅ Afficher un résumé de l'installation

**Durée estimée** : 5-10 minutes (selon votre connexion internet)

---

### Option 2 : Installation Manuelle

Si vous préférez contrôler chaque étape :

#### Étape 1 : Cloner le Repository

```bash
git clone https://github.com/VOTRE_USERNAME/ONEE.SSO.git
cd ONEE.SSO
```

#### Étape 2 : Créer la Base de Données

```powershell
# Exécuter le script SQL de création
sqlcmd -S localhost\SQLEXPRESS -i scripts/database/CREATE_DATABASE.sql
```

#### Étape 3 : Configurer le Backend (SSO)

```powershell
cd src/ONEE.SSO.API

# Copier le fichier de configuration exemple
Copy-Item appsettings.example.json appsettings.json

# ⚠️ IMPORTANT : Modifier appsettings.json avec vos valeurs
# - Changer la ConnectionString si nécessaire
# - Générer un nouveau JWT Secret (minimum 32 caractères)
# - Configurer les Client Secrets

# Appliquer les migrations
dotnet ef database update --project ../ONEE.SSO.Infrastructure

# Compiler le projet
dotnet build
```

#### Étape 4 : Installer les Dépendances Frontends

```powershell
# Client RH
cd clients/rh/frontend
npm install

# Client TIMS
cd ../../tims/frontend
npm install

# Client EAMS
cd ../../eams/frontend
npm install
```

---

## ⚙️ Configuration

### Configuration du SSO (`src/ONEE.SSO.API/appsettings.json`)

```json
{
  "Database": {
    "ConnectionString": "Server=VOTRE_SERVEUR\\SQLEXPRESS;Database=ONEE_SSO;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  
  "Jwt": {
    "Secret": "VOTRE_SECRET_JWT_MINIMUM_32_CARACTERES",
    "Issuer": "https://localhost:5205",
    "Audience": "https://localhost:5205",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  
  "ClientApplications": [
    {
      "ClientId": "rh_client",
      "ClientSecret": "VOTRE_SECRET_CLIENT_RH",
      "RedirectUris": ["http://localhost:5174/auth/callback"],
      "PostLogoutRedirectUris": ["http://localhost:5174"],
      "AllowedScopes": ["openid", "profile", "email", "roles"]
    }
    // ... autres clients
  ]
}
```

### ⚠️ Variables à Changer Obligatoirement

Avant de déployer en production, **CHANGEZ** :

1. **JWT Secret** : Générez une chaîne aléatoire de 32+ caractères
   ```powershell
   # Générer un secret aléatoire
   -join ((48..57) + (65..90) + (97..122) | Get-Random -Count 32 | ForEach-Object {[char]$_})
   ```

2. **Client Secrets** : Pour chaque application cliente

3. **ConnectionString** : Adaptez à votre serveur SQL

4. **Mot de passe admin** : Par défaut `Admin@123` (voir `ClientApplicationsSeeder.cs`)

---

## 🎮 Utilisation

### Démarrer Toutes les Applications

```powershell
# À la racine du projet
.\START_ALL.ps1
```

**Ce script démarre automatiquement :**
- ✅ Backend SSO (port 5205)
- ✅ Client RH (port 5174)
- ✅ Client TIMS (port 5175)
- ✅ Client EAMS (port 5173)

**Temps de démarrage** : ~30 secondes

---

### Arrêter Toutes les Applications

```powershell
.\STOP_ALL.ps1
```

---

### Démarrage Manuel (Pour Développement)

#### Backend SSO

```powershell
cd src/ONEE.SSO.API
dotnet run
```
- URL : `https://localhost:5205`
- Swagger : `https://localhost:5205/swagger`

#### Client RH

```powershell
cd clients/rh/frontend
npm run dev
```
- URL : `http://localhost:5174`

#### Client TIMS

```powershell
cd clients/tims/frontend
npm run dev
```
- URL : `http://localhost:5175`

#### Client EAMS

```powershell
cd clients/eams/frontend
npm run dev
```
- URL : `http://localhost:5173`

---

### Accès aux Applications

| Application | URL | Identifiants par Défaut |
|------------|-----|------------------------|
| **SSO Admin** | https://localhost:5205 | `admin@onee.ma` / `Admin@123` |
| **Client RH** | http://localhost:5174 | `user.multi@onee.ma` / `User123!` |
| **Client TIMS** | http://localhost:5175 | `user.multi@onee.ma` / `User123!` |
| **Client EAMS** | http://localhost:5173 | `user.multi@onee.ma` / `User123!` |

#### Utilisateurs de Test

| Email | Mot de Passe | Accès Applications |
|-------|-------------|-------------------|
| `admin@onee.ma` | `Admin@123` | Toutes (Admin SSO) |
| `user.multi@onee.ma` | `User123!` | RH + TIMS + EAMS |
| `user.partiel@onee.ma` | `User123!` | RH + EAMS uniquement |

---

## 📁 Structure du Projet

```
ONEE.SSO/
│
├── 📁 src/                          # Microservice SSO
│   ├── ONEE.SSO.API/                # Couche API (Controllers, Middleware)
│   ├── ONEE.SSO.Application/        # Couche Application (Services, DTOs)
│   ├── ONEE.SSO.Domain/             # Couche Domain (Entities, Enums)
│   └── ONEE.SSO.Infrastructure/     # Couche Infrastructure (DbContext, Repos)
│
├── 📁 clients/                      # Applications clientes
│   ├── rh/                          # Client RH
│   │   ├── backend/                 # API RH (ASP.NET Core)
│   │   └── frontend/                # UI RH (React + TypeScript)
│   ├── tims/                        # Client TIMS
│   │   ├── backend/                 # API TIMS
│   │   └── frontend/                # UI TIMS
│   └── eams/                        # Client EAMS
│       ├── backend/                 # API EAMS
│       └── frontend/                # UI EAMS
│
├── 📁 scripts/                      # Scripts SQL et automatisation
│   └── database/                    # Scripts SQL (CREATE, RESET, etc.)
│
├── 📁 .github/                      # Configuration GitHub
│   └── workflows/                   # CI/CD (si configuré)
│
├── 📄 SETUP_COMPLET.ps1             # Installation automatique
├── 📄 START_ALL.ps1                 # Démarrer toutes les apps
├── 📄 STOP_ALL.ps1                  # Arrêter toutes les apps
├── 📄 RESET_AND_RESTART.ps1         # Reset BDD + Redémarrage
│
├── 📄 README.md                     # Ce fichier
├── 📄 CONTRIBUTING.md               # Guide de contribution
├── 📄 SECURITY.md                   # Guide de sécurité
├── 📄 LICENSE                       # Licence MIT
├── 📄 .gitignore                    # Fichiers ignorés par Git
└── 📄 ONEE.SSO.sln                  # Solution Visual Studio
```

---

## 🔒 Sécurité

### ⚠️ Avertissements Importants

Ce projet contient des **secrets de développement** à des fins de démonstration.

**AVANT TOUT DÉPLOIEMENT EN PRODUCTION** :

1. ✅ **Changer le JWT Secret**
   - Fichier : `src/ONEE.SSO.API/appsettings.json`
   - Générer une clé sécurisée de 32+ caractères

2. ✅ **Changer les Client Secrets OIDC**
   - Fichier : `src/ONEE.SSO.Infrastructure/Data/Seeders/ClientApplicationsSeeder.cs`
   - Hasher avec BCrypt avant insertion

3. ✅ **Changer le mot de passe admin**
   - Par défaut : `Admin@123`
   - Créer un mot de passe fort et unique

4. ✅ **Configurer HTTPS en production**
   - Certificat SSL valide
   - HSTS activé

5. ✅ **Restreindre les CORS**
   - Autoriser uniquement les domaines de production

6. ✅ **Activer les logs de sécurité**
   - Surveiller les tentatives de connexion
   - Alertes sur activités suspectes

### Bonnes Pratiques Implémentées

- ✅ **Hashage BCrypt** (work factor 12)
- ✅ **Tokens JWT signés** (HMAC-SHA256)
- ✅ **Refresh tokens** avec rotation
- ✅ **Protection force brute** (verrouillage après 5 tentatives)
- ✅ **PKCE obligatoire** pour les flows OAuth
- ✅ **Claims validation** sur chaque requête
- ✅ **Audit logs** de toutes les actions
- ✅ **Middleware d'exception** pour ne pas exposer les détails techniques

### Signaler une Vulnérabilité

Si vous découvrez une faille de sécurité, **NE CRÉEZ PAS** d'issue publique.

Contactez-nous : **security@onee.ma**

Voir [SECURITY.md](./SECURITY.md) pour plus de détails.

---

## 🤝 Contribution

Les contributions sont les bienvenues ! Consultez [CONTRIBUTING.md](./CONTRIBUTING.md) pour :

- 🐛 Signaler un bug
- 💡 Proposer une fonctionnalité
- 🔧 Soumettre une Pull Request
- 📖 Améliorer la documentation

### Contributeurs

- **Salma** - *Développement initial* - [EMSI Casablanca](https://www.emsi.ma/)

---

## 📝 Licence

Ce projet est sous licence **MIT** - voir le fichier [LICENSE](./LICENSE) pour plus de détails.

```
MIT License

Copyright (c) 2026 Salma - ONEE

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction...
```

---

## 👨‍💻 Auteur

**Salma Hmirin**   


## 📞 Contact & Support

- 📧 **Email** : hmirinsalmaa@gmail.com
- 🐛 **Issues** : [GitHub Issues](https://github.com/VOTRE_USERNAME/ONEE.SSO/issues)
- 📖 **Documentation** : [Wiki](https://github.com/VOTRE_USERNAME/ONEE.SSO/wiki) (si configuré)

---



## 📊 Statistiques du Projet

- **Lignes de code** : ~15,000+ lignes
- **Durée de développement** : 3 mois (Juillet - Septembre 2026)
- **Technologies utilisées** : 10+
- **Applications complètes** : 4 (SSO + 3 clients)
- **Endpoints API** : 50+

---

<div align="center">

**Développé avec ❤️ par Salma**

⭐ **Si ce projet vous aide, n'hésitez pas à lui donner une étoile !** ⭐

</div>
