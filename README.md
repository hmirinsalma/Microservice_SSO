# 🔐 ONEE SSO - Système d'Authentification Unique

> Projet de fin d'études - Système SSO complet avec 3 applications clientes intégrées

---

## 📋 Table des Matières

- [Vue d'ensemble](#vue-densemble)
- [Architecture](#architecture)
- [Démarrage Rapide](#démarrage-rapide)
- [Structure du Projet](#structure-du-projet)
- [Applications](#applications)
- [Configuration](#configuration)
- [Documentation](#documentation)

---

## 🎯 Vue d'ensemble

Ce projet implémente un système SSO (Single Sign-On) complet basé sur le protocole **OpenID Connect (OIDC)** avec 3 applications clientes :

1. **Gestion du Personnel (RH)** - Système de gestion des ressources humaines
2. **TIMS** - Gestion des interventions techniques
3. **EAMS** - Gestion des équipements et actifs

### Fonctionnalités principales

✅ **Authentification centralisée** via OIDC/OAuth2  
✅ **Gestion des rôles et permissions** (RBAC)  
✅ **Session unique** entre toutes les applications  
✅ **Interface web moderne** avec Razor Pages  
✅ **Consentement utilisateur** avant autorisation  
✅ **Support PKCE** pour sécurité renforcée  
✅ **Refresh tokens** pour sessions longues durées  
✅ **Audit logs** complets  

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    ONEE SSO Server (5205)                   │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │   Login      │  │  Authorize   │  │    Token     │     │
│  │   Logout     │  │   Consent    │  │   Endpoint   │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
└─────────────────────────────────────────────────────────────┘
                            │
        ┌───────────────────┼───────────────────┐
        │                   │                   │
┌───────▼───────┐   ┌──────▼──────┐   ┌───────▼───────┐
│  RH (5173)    │   │ TIMS (5175) │   │ EAMS (5173)   │
│  Frontend     │   │  Frontend   │   │  Frontend     │
│  Backend 5291 │   │  Backend    │   │  Backend 5137 │
└───────────────┘   │  5115       │   └───────────────┘
                    └─────────────┘
```

### Technologies utilisées

**Backend SSO** :
- ASP.NET Core 9.0
- Entity Framework Core
- SQL Server
- JWT Bearer Authentication
- Serilog pour logging

**Frontends** :
- React 19 (RH & TIMS)
- React 19 + TypeScript (EAMS)
- Vite
- oidc-client-ts
- Tailwind CSS

**Backends Applications** :
- ASP.NET Core 9.0
- Entity Framework Core
- SQL Server

---

## 🚀 Démarrage Rapide

### Prérequis

- **Node.js** 18+ et npm
- **.NET SDK** 9.0
- **SQL Server** (LocalDB ou Express)
- **PowerShell** 7+

### Lancer toutes les applications (Méthode 1 - AUTOMATIQUE ✅)

```powershell
cd "c:\Users\XPS\source\repos\ONEE.SSO"
.\START_ALL.ps1
```

Ce script lance automatiquement :
- ✅ Le serveur SSO (port 5205)
- ✅ Les 3 frontends (RH, TIMS, EAMS)
- ✅ Les 3 backends (RH, TIMS, EAMS)

### Lancer manuellement (Méthode 2)

#### 1. SSO Server
```powershell
cd src\ONEE.SSO.API
dotnet run
```

#### 2. Gestion Personnel (RH)
```powershell
# Backend
cd clients\gestion-personnel\backend
dotnet run

# Frontend (nouvelle fenêtre)
cd clients\gestion-personnel\frontend
npm run dev
```

#### 3. TIMS
```powershell
# Backend
cd clients\tims\backend
dotnet run

# Frontend (nouvelle fenêtre)
cd clients\tims\frontend
npm run dev
```

#### 4. EAMS
```powershell
# Backend
cd clients\eams\backend
dotnet run

# Frontend (nouvelle fenêtre)
cd clients\eams\frontend
npm run dev
```

---

## 📂 Structure du Projet

```
ONEE.SSO/
│
├── src/                              # Code source SSO
│   ├── ONEE.SSO.API/                # API principale (port 5205)
│   │   ├── Controllers/             # Endpoints API
│   │   ├── Pages/                   # Razor Pages (Login, Consent, etc.)
│   │   ├── Services/                # Services métier
│   │   └── wwwroot/                 # CSS, JS, images
│   │
│   ├── ONEE.SSO.Application/        # Logique métier
│   ├── ONEE.SSO.Domain/             # Entités et interfaces
│   ├── ONEE.SSO.Infrastructure/     # Data access, seeders
│   └── ONEE.SSO.Shared/             # Code partagé
│
├── clients/                          # Applications clientes
│   ├── gestion-personnel/           # RH
│   │   ├── frontend/                # React (port 5173)
│   │   └── backend/                 # .NET (port 5291)
│   │
│   ├── tims/                        # Gestion Interventions
│   │   ├── frontend/                # React (port 5175)
│   │   └── backend/                 # .NET (port 5115)
│   │
│   └── eams/                        # Gestion Équipements
│       ├── frontend/                # React+TS (port 5173)
│       └── backend/                 # .NET (port 5137)
│
├── docs/                            # Documentation
├── START_ALL.ps1                    # Script de démarrage automatique
└── README.md                        # Ce fichier
```

---

## 🌐 Applications

### 1. SSO Server (Port 5205)

**URLs** :
- Interface web : `http://localhost:5205`
- Login : `http://localhost:5205/Login`
- API : `http://localhost:5205/api/`
- Swagger : `http://localhost:5205/swagger`

**Endpoints OIDC** :
- Authorization : `/connect/authorize`
- Token : `/connect/token`
- UserInfo : `/api/auth/userinfo`
- Logout : `/Logout`

### 2. Gestion du Personnel (RH)

**URLs** :
- Frontend : `http://localhost:5173`
- Backend API : `http://localhost:5291`

**Fonctionnalités** :
- Gestion des employés
- Gestion des départements
- Gestion des contrats
- Gestion des congés

### 3. TIMS - Gestion des Interventions

**URLs** :
- Frontend : `http://localhost:5175`
- Backend API : `http://localhost:5115`

**Fonctionnalités** :
- Gestion des tickets
- Planification des interventions
- Suivi des équipes
- Rapports d'intervention

### 4. EAMS - Gestion des Équipements

**URLs** :
- Frontend : `http://localhost:5173`
- Backend API : `http://localhost:5137`

**Fonctionnalités** :
- Inventaire des équipements
- Maintenance préventive
- Historique des pannes
- Gestion des pièces détachées

---

## ⚙️ Configuration

### Identifiants de test

```
Email:    admin@onee.ma
Password: Admin@123
```

### Ports utilisés

| Service | Port | URL |
|---------|------|-----|
| SSO Server | 5205 | http://localhost:5205 |
| RH Frontend | 5173 | http://localhost:5173 |
| RH Backend | 5291 | http://localhost:5291 |
| TIMS Frontend | 5175 | http://localhost:5175 |
| TIMS Backend | 5115 | http://localhost:5115 |
| EAMS Frontend | 5173 | http://localhost:5173 |
| EAMS Backend | 5137 | http://localhost:5137 |

### Bases de données

Toutes les bases utilisent **SQL Server LocalDB** :
- `ONEE_SSO` - Base SSO principale
- `GestionPersonnel_DB` - Base RH
- `TIMS_DB` - Base TIMS
- `EAMS_DB` - Base EAMS

---

## 📚 Documentation

### Flux SSO Complet

1. **User** accède à une application cliente (ex: RH)
2. **Application** redirige vers `/connect/authorize` (SSO)
3. **SSO** affiche la page de login
4. **User** saisit email/password
5. **SSO** affiche la page de consentement
6. **User** clique "Autoriser"
7. **SSO** génère un code d'autorisation
8. **Application** reçoit le code via callback
9. **Application** échange le code contre un token (`/connect/token`)
10. **SSO** retourne access_token + refresh_token
11. **Application** stocke le token et redirige vers le dashboard
12. **User** est authentifié ✅

### Sécurité

- ✅ **PKCE** (Proof Key for Code Exchange) activé
- ✅ **Hachage BCrypt** pour les mots de passe
- ✅ **JWT** avec signature HMAC-SHA256
- ✅ **HTTPS** recommandé en production
- ✅ **CORS** configuré
- ✅ **Tokens expirables** (access: 15-60min, refresh: 1-30 jours)

### Rôles et Permissions

**Rôles disponibles** :
- SuperAdmin
- AdministrateurRH
- AdministrateurTIMS
- AdministrateurEAMS
- UtilisateurRH
- UtilisateurTIMS
- UtilisateurEAMS

**Permissions** :
- Gestion des utilisateurs
- Gestion des rôles
- Gestion des applications
- Consultation des logs
- etc.

---

## 🛠️ Développement

### Compiler le SSO

```powershell
cd src\ONEE.SSO.API
dotnet build
```

### Réinitialiser la base de données

```powershell
cd src\ONEE.SSO.API
dotnet ef database drop --force
dotnet ef database update
```

### Logs

Les logs sont stockés dans :
- SSO : `src/ONEE.SSO.API/Logs/`
- Format : `log-YYYYMMDD.txt`

---

## 📊 Statistiques du Projet

- **Lignes de code** : ~15,000+
- **Fichiers** : 200+
- **Entités** : 15+
- **Endpoints API** : 50+
- **Pages Razor** : 6
- **Applications clientes** : 3

---

## 👨‍💻 Auteur

**Projet de fin d'études ONEE**  
Développé par : [Votre Nom]  
Date : 2026

---

## 📝 License

© 2026 ONEE - Tous droits réservés

---

## 🆘 Support

Pour toute question ou problème :
1. Vérifier que tous les serveurs sont lancés
2. Vérifier les logs dans `src/ONEE.SSO.API/Logs/`
3. Vérifier la console du navigateur (F12)
4. Vérifier les ports (pas de conflits)

---

**🎉 Bon développement !**
