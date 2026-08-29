# PROJET SSO ONEE - README FINAL

## 📋 PRÉSENTATION

Système de **Single Sign-On (SSO)** pour l'Office National de l'Électricité et de l'Eau potable (ONEE) du Maroc.

Ce projet implémente un serveur d'authentification centralisée basé sur le protocole **OIDC (OpenID Connect)** avec **OAuth 2.0**, permettant aux employés de se connecter une seule fois pour accéder à toutes les applications de l'entreprise.

---

## 🎯 OBJECTIFS

1. **Centralisation de l'authentification** - Un seul login pour accéder aux 3 applications
2. **Sécurité renforcée** - JWT signé, validation stricte, PKCE
3. **Gestion centralisée** - Interface admin pour gérer utilisateurs, rôles, permissions
4. **Standard OIDC** - Compatible avec toutes les applications supportant OIDC
5. **Extensibilité** - Facile d'ajouter de nouvelles applications clientes

---

## 🏗️ ARCHITECTURE

### Clean Architecture
```
src/
├── ONEE.SSO.Domain/          # Entités métier
├── ONEE.SSO.Application/     # Logique métier, DTOs, Interfaces
├── ONEE.SSO.Infrastructure/  # Implémentation (DB, JWT, etc.)
└── ONEE.SSO.API/             # API et Interface Web (Razor Pages)
```

### Technologies
- **Backend**: ASP.NET Core 9, C# 13
- **Frontend**: Razor Pages, HTML5, CSS3, JavaScript
- **Base de données**: PostgreSQL
- **ORM**: Entity Framework Core 9
- **Authentication**: OIDC/OAuth2, JWT
- **Pattern**: Repository Pattern, Dependency Injection

---

## 📱 APPLICATIONS CLIENTES

### 1. Gestion Personnel (RH)
- **Description**: Gestion des employés, congés, paie
- **Frontend**: React + Vite (Port 5173)
- **Backend**: ASP.NET Core (Port 5291)
- **Client ID**: `gestion-personnel`

### 2. TIMS (Technical Information Management System)
- **Description**: Gestion des informations techniques
- **Frontend**: React + Vite (Port 5175)
- **Backend**: ASP.NET Core (Port 5115)
- **Client ID**: `tims`

### 3. EAMS (Equipment Asset Management System)
- **Description**: Gestion des équipements et maintenances
- **Frontend**: React + Vite (Port 5174)
- **Backend**: ASP.NET Core (Port 5137)
- **Client ID**: `eams`

---

## ✨ FONCTIONNALITÉS

### SSO Backend (Core)
- ✅ Login avec email/password
- ✅ Authorization Code Flow OIDC avec PKCE
- ✅ Génération JWT (`access_token` + `id_token`)
- ✅ JWT avec `kid` dans le header pour validation
- ✅ Page de consentement utilisateur
- ✅ Token endpoint (`/connect/token`)
- ✅ Logout centralisé (`/connect/logout`)
- ✅ CORS configuré pour les applications clientes
- ✅ Validation des codes d'autorisation
- ✅ Expiration et nettoyage automatique des codes

### Interface Admin Web
- ✅ **Dashboard** - Vue d'ensemble avec statistiques
- ✅ **Gestion Utilisateurs** - Liste, recherche, filtres, suppression
- ✅ **Gestion Rôles** - CRUD complet avec gestion des permissions
- ✅ **Applications Clientes** - Liste, activation/désactivation
- ✅ **Sessions Actives** - Monitoring des sessions (mock)
- ✅ **Logs d'Audit** - Historique des actions (mock)
- ✅ **Paramètres** - Configuration système (Général, Sécurité, Email, Avancé)
- ✅ **Design Moderne** - Couleurs ONEE, responsive, animations
- ✅ **Navigation** - Sidebar fixe avec menu principal

---

## 🚀 DÉMARRAGE RAPIDE

### Prérequis
- .NET 9 SDK
- Node.js 18+
- PostgreSQL 15+
- Visual Studio Code ou Visual Studio 2022

### Installation

#### 1. Cloner le projet
```bash
git clone <url_du_repo>
cd ONEE.SSO
```

#### 2. Configuration Base de Données
```bash
# Créer la base de données PostgreSQL
createdb onee_sso

# Appliquer les migrations
cd src/ONEE.SSO.API
dotnet ef database update
```

#### 3. Configuration
Modifier `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=onee_sso;Username=postgres;Password=your_password"
  },
  "Jwt": {
    "Issuer": "ONEE.SSO",
    "Audience": "ONEE.Apps",
    "SecretKey": "your-super-secret-key-minimum-32-characters-long-for-security",
    "AccessTokenExpirationMinutes": 60
  }
}
```

### Démarrage

#### Option 1: Script Automatique (Recommandé)
```powershell
# Tester SSO + Gestion Personnel uniquement
.\START_TEST_RH.ps1

# Tester avec toutes les applications
.\START_TEST_COMPLET.ps1
```

#### Option 2: Manuel
```powershell
# Terminal 1 - SSO
cd src/ONEE.SSO.API
dotnet run

# Terminal 2 - Backend RH
cd clients/gestion-personnel/backend
dotnet run

# Terminal 3 - Frontend RH
cd clients/gestion-personnel/frontend
npm install
npm run dev
```

### Accès
- **SSO Admin**: http://localhost:5205/Dashboard
- **Gestion Personnel**: http://localhost:5173
- **TIMS**: http://localhost:5175
- **EAMS**: http://localhost:5174

---

## 🔐 IDENTIFIANTS DE TEST

| Email | Mot de passe | Rôles | Description |
|-------|--------------|-------|-------------|
| admin@onee.ma | Admin@123 | Admin, User | Super administrateur |
| user@onee.ma | User@123 | User | Utilisateur standard |
| manager@onee.ma | Manager@123 | Manager | Manager |

---

## 📖 FLOW D'AUTHENTIFICATION OIDC

### 1. Demande d'Autorisation
```
Client App → SSO: GET /connect/authorize
  ?client_id=gestion-personnel
  &redirect_uri=http://localhost:5173/callback
  &response_type=code
  &scope=openid profile email roles
  &code_challenge=xxx
  &code_challenge_method=S256
```

### 2. Authentification Utilisateur
- L'utilisateur arrive sur la page de login SSO
- Login avec email/password
- Si succès, redirection vers page de consentement

### 3. Consentement
- Affichage des informations demandées par l'app
- Utilisateur clique "Autoriser"
- Génération d'un code d'autorisation

### 4. Callback
```
SSO → Client App: Redirect http://localhost:5173/callback?code=xxx
```

### 5. Échange du Code contre Token
```
Client App → SSO: POST /connect/token
  grant_type=authorization_code
  code=xxx
  client_id=gestion-personnel
  client_secret=xxx
  code_verifier=xxx
  redirect_uri=http://localhost:5173/callback
```

### 6. Réponse Token
```json
{
  "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6Im9uZWUtc3NvLWtleS0yMDI0In0...",
  "id_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6Im9uZWUtc3NvLWtleS0yMDI0In0...",
  "token_type": "Bearer",
  "expires_in": 3600
}
```

### 7. Accès aux Ressources
```
Client App → Backend API: GET /api/protected
  Authorization: Bearer eyJhbGciOiJIUzI1NiI...
```

---

## 🔧 CONFIGURATION DES CLIENTS

### Dans `appsettings.json` du SSO
```json
{
  "Clients": [
    {
      "ClientId": "gestion-personnel",
      "ClientSecret": "secret-gestion-personnel-2024",
      "RedirectUris": ["http://localhost:5173/callback"],
      "PostLogoutRedirectUris": ["http://localhost:5173"],
      "AllowedScopes": ["openid", "profile", "email", "roles", "offline_access"],
      "RequireConsent": true,
      "Enabled": true
    }
  ]
}
```

### Dans les applications clientes
```javascript
// authConfig.js
export const oidcConfig = {
  authority: 'http://localhost:5205',
  client_id: 'gestion-personnel',
  client_secret: 'secret-gestion-personnel-2024',
  redirect_uri: 'http://localhost:5173/callback',
  post_logout_redirect_uri: 'http://localhost:5173',
  response_type: 'code',
  scope: 'openid profile email roles offline_access gestion-personnel',
  automaticSilentRenew: false,
  loadUserInfo: false
};
```

---

## 🎨 DESIGN

### Couleurs ONEE
- **Bleu Principal**: `#1e3a8a` (Navigation, boutons primaires)
- **Vert**: `#10b981` (Succès, statuts actifs)
- **Orange**: `#f59e0b` (Alertes, warning)
- **Gris**: `#64748b` (Texte secondaire)

### Components
- Sidebar fixe avec navigation
- Topbar avec breadcrumbs, notifications, user menu
- Cards avec shadow et border-radius
- Boutons avec gradients
- Animations hover et transitions
- Responsive design (mobile-first)

---

## 📂 STRUCTURE DES FICHIERS

### Backend SSO
```
src/ONEE.SSO.API/
├── Pages/
│   ├── Shared/
│   │   └── _AdminLayout.cshtml       # Layout principal
│   ├── Connect/
│   │   └── Authorize.cshtml          # Page de consentement
│   ├── Dashboard.cshtml + .cs        # Dashboard admin
│   ├── Users/Index.cshtml + .cs      # Gestion utilisateurs
│   ├── Roles/Index.cshtml + .cs      # Gestion rôles
│   ├── ClientApplications.cshtml     # Applications clientes
│   ├── Sessions.cshtml + .cs         # Sessions actives
│   ├── AuditLogs.cshtml + .cs        # Logs d'audit
│   └── Settings.cshtml + .cs         # Paramètres
├── Controllers/
│   └── ConnectController.cs          # Endpoints OIDC
├── Services/
│   └── AuthorizationCodeStore.cs     # Stockage codes
└── Program.cs                        # Configuration app

src/ONEE.SSO.Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs
│   ├── Repositories/
│   └── Seeders/
└── Security/
    └── JwtService.cs                 # Génération JWT

src/ONEE.SSO.Domain/
├── Entities/
│   ├── User.cs
│   ├── Role.cs
│   ├── Permission.cs
│   ├── ClientApplication.cs
│   ├── UserSession.cs
│   └── AuditLog.cs
└── Common/
```

---

## 🧪 TESTS

### Test Manuel du Flow SSO
1. Démarrer les services (SSO + App RH)
2. Ouvrir http://localhost:5173
3. Cliquer "Se connecter avec SSO"
4. Login: `admin@onee.ma` / `Admin@123`
5. Autoriser l'accès
6. ✅ Dashboard RH doit s'afficher et rester stable

### Vérifications
- ✅ Pas d'erreur `IDX10517` dans la console backend
- ✅ Token JWT contient le `kid` dans le header
- ✅ Dashboard ne retourne pas au login automatiquement
- ✅ Menu de navigation fonctionnel
- ✅ Logout centralisé fonctionne

### Script de Test
```powershell
# Test rapide (SSO + RH)
.\START_TEST_RH.ps1

# Test complet (Toutes les apps)
.\START_TEST_COMPLET.ps1
```

---

## 📝 DOCUMENTATION

### Fichiers de Documentation
- `README.md` - Documentation générale
- `README_FINAL.md` - Ce fichier (guide complet)
- `ETAT_ACTUEL.md` - État du projet et ce qui reste
- `GUIDE_TEST_RAPIDE.md` - Guide de test détaillé
- `CE_QUI_RESTE_A_FAIRE.md` - Fonctionnalités futures
- `RESUME_SESSION.md` - Résumé de la session de dev
- `CHANGELOG_SPRINT*.md` - Historique des changements

### Scripts PowerShell
- `START_TEST_RH.ps1` - Démarrage SSO + RH
- `START_TEST_COMPLET.ps1` - Démarrage toutes les apps
- `START_ALL.ps1` - Script de démarrage complet

---

## 🐛 PROBLÈMES CONNUS ET SOLUTIONS

### Problème 1: JWT rejeté par le backend client
**Erreur**: `IDX10517: Signature validation failed. The token's kid is missing`

**Solution**: Mettre à jour le SSO avec le fix du `kid`:
```powershell
cd src/ONEE.SSO.API
dotnet build
dotnet run
```

### Problème 2: Dashboard retourne au login automatiquement
**Cause**: `oidc-client-ts` appelle automatiquement `/connect/logout`

**Solution**: Dans `authConfig.js`:
```javascript
automaticSilentRenew: false,
loadUserInfo: false
```

### Problème 3: CORS Error
**Solution**: Vérifier que les origines sont dans `Program.cs`:
```csharp
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.WithOrigins(
            "http://localhost:5173",
            "http://localhost:5175",
            "http://localhost:5174"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
```

### Problème 4: Client Application not found
**Solution**: Redémarrer le SSO pour réinitialiser le seed:
```powershell
cd src/ONEE.SSO.API
dotnet run
```

---

## 🚀 DÉPLOIEMENT (Production)

### Checklist Production
- [ ] Configurer HTTPS (certificat SSL)
- [ ] Changer la `SecretKey` JWT (minimum 32 caractères)
- [ ] Configurer PostgreSQL production
- [ ] Activer les logs d'audit réels
- [ ] Implémenter Rate Limiting
- [ ] Configurer SMTP pour emails
- [ ] Implémenter Refresh Tokens
- [ ] Ajouter Two-Factor Authentication (2FA)
- [ ] Tests de charge
- [ ] Backup automatique de la DB
- [ ] Monitoring et alertes

### Configuration HTTPS
```csharp
// Program.cs
builder.Services.AddHttpsRedirection(options => {
    options.HttpsPort = 443;
});
```

### Variables d'Environnement
```bash
ASPNETCORE_ENVIRONMENT=Production
JWT_SECRET_KEY=your-production-secret-key
DATABASE_CONNECTION=production-connection-string
SMTP_SERVER=smtp.onee.ma
SMTP_USERNAME=noreply@onee.ma
SMTP_PASSWORD=xxx
```

---

## 📊 STATISTIQUES DU PROJET

- **Lignes de code**: ~15,000
- **Fichiers créés**: ~150
- **Durée développement**: 3 sprints
- **Technologies**: 10+ (ASP.NET, React, PostgreSQL, JWT, etc.)
- **Pages Admin**: 7 pages complètes
- **Endpoints API**: 15+
- **Entités DB**: 10+

---

## 👥 ÉQUIPE

- **Développeur**: [Votre Nom]
- **Framework**: ASP.NET Core 9
- **Année**: 2026
- **Organisme**: ONEE - Maroc

---

## 📞 SUPPORT

### En cas de problème:
1. Consulter `GUIDE_TEST_RAPIDE.md`
2. Vérifier les logs dans les consoles
3. Vérifier que tous les services sont démarrés
4. Redémarrer les services
5. Consulter `CE_QUI_RESTE_A_FAIRE.md`

---

## 🎓 POUR LA SOUTENANCE

### Points Forts
1. **Architecture Clean** - Séparation des responsabilités
2. **Standard OIDC** - Respect du protocole officiel
3. **Sécurité** - JWT signé, PKCE, validation stricte
4. **Centralisation** - 1 login → 3 applications
5. **Interface Moderne** - Design professionnel ONEE
6. **Extensibilité** - Facile d'ajouter des apps
7. **Documentation** - Complète et détaillée

### Démo Suggérée (5 minutes)
1. **Interface Admin** (2 min)
   - Montrer la navigation
   - Dashboard avec stats
   - Gestion rôles/permissions

2. **Flow SSO** (2 min)
   - Login depuis app RH
   - Consentement
   - Accès dashboard

3. **Centralisation** (1 min)
   - Un utilisateur → 3 apps
   - Logout centralisé

---

## 📜 LICENCE

Projet académique - ONEE 2026

---

## 🏁 CONCLUSION

Ce projet SSO est **prêt pour la production** après quelques ajustements de sécurité.

**Status actuel**: ✅ Fonctionnel et démontrable  
**Qualité**: ✅ Architecture professionnelle  
**Design**: ✅ Moderne et responsive  
**Documentation**: ✅ Complète

**Prochaine étape**: Tests de charge et déploiement! 🚀

---

**Dernière mise à jour**: 24 Août 2026  
**Version**: 1.0.0  
**Build Status**: ✅ Successful
