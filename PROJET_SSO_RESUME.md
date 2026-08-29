# PROJET SSO ONEE - RÉSUMÉ COMPLET

## 🎯 OBJECTIF DU PROJET
Développer un système d'authentification centralisé (Single Sign-On) pour les applications internes de l'ONEE, permettant aux utilisateurs de se connecter une seule fois pour accéder à toutes les applications.

## 📋 APPLICATIONS INTÉGRÉES

### 1. **Gestion Personnel (RH)**
- **Frontend**: React 19 + Vite (port 5173)
- **Backend**: ASP.NET Core 9 (port 5291)
- **Client ID**: `gestion-personnel`
- **Fonctionnalités**: Gestion des employés, congés, directions, services

### 2. **TIMS** (Gestion des Interventions)
- **Frontend**: React (port 5175)
- **Backend**: ASP.NET Core (port 5115)
- **Client ID**: `tims-app`
- **Fonctionnalités**: Gestion des tickets d'intervention technique

### 3. **EAMS** (Gestion des Équipements)
- **Frontend**: React + TypeScript (port 5173)
- **Backend**: ASP.NET Core (port 5137)
- **Client ID**: `eams-spa`
- **Fonctionnalités**: Gestion des équipements et maintenance

## 🏗️ ARCHITECTURE TECHNIQUE

### Backend SSO (Port 5205)
```
ONEE.SSO/
├── src/
│   ├── ONEE.SSO.API/              # API principale + Pages Razor
│   │   ├── Pages/
│   │   │   ├── Login.cshtml       # Page de connexion SSO
│   │   │   ├── Logout.cshtml      # Page de déconnexion
│   │   │   ├── Dashboard.cshtml   # Dashboard admin SSO
│   │   │   ├── Connect/
│   │   │   │   └── Authorize.cshtml  # Page de consentement
│   │   ├── Controllers/
│   │   │   └── ConnectController.cs  # Endpoints OIDC (/connect/token, /connect/logout)
│   │   ├── Services/
│   │   │   └── AuthorizationCodeStore.cs  # Stockage des codes d'autorisation
│   │   └── wwwroot/
│   │       └── css/onee-theme.css    # Design ONEE
│   ├── ONEE.SSO.Application/      # Business Logic
│   │   ├── Interfaces/
│   │   │   ├── IJwtService.cs
│   │   │   └── IPasswordHasher.cs
│   │   └── Repositories/
│   │       ├── IUserRepository.cs
│   │       ├── IRoleRepository.cs
│   │       └── IClientApplicationRepository.cs
│   ├── ONEE.SSO.Infrastructure/   # Implémentations
│   │   ├── Security/
│   │   │   ├── JwtService.cs      # Génération access_token + id_token
│   │   │   └── PasswordHasher.cs
│   │   └── Persistence/
│   │       ├── AppDbContext.cs
│   │       └── Seed/
│   │           ├── ClientApplicationsSeeder.cs
│   │           ├── RolesSeeder.cs
│   │           └── UsersSeeder.cs
│   └── ONEE.SSO.Domain/           # Entités
│       ├── User.cs
│       ├── Role.cs
│       ├── Permission.cs
│       └── ClientApplication.cs
```

### Base de données (SQL Server)
```sql
-- Principales tables
Users               -- Utilisateurs SSO
Roles               -- Rôles (Admin, Manager, Employe, etc.)
Permissions         -- Permissions (users.read, users.write, etc.)
UserRoles           -- Association Users <-> Roles
RolePermissions     -- Association Roles <-> Permissions
ClientApplications  -- Applications clientes (RH, TIMS, EAMS)
```

## 🔐 FLOW D'AUTHENTIFICATION OIDC

### Étape 1: Demande d'autorisation
```
Client (RH Frontend)
    ↓
    GET /connect/authorize?
        client_id=gestion-personnel&
        redirect_uri=http://localhost:5173/callback&
        response_type=code&
        scope=openid profile email roles&
        code_challenge=xxx&
        code_challenge_method=S256
    ↓
SSO Login Page
```

### Étape 2: Authentification
```
User entre: admin@onee.ma / Admin@123
    ↓
SSO valide credentials
    ↓
Génère Access Token JWT (stocké en session)
```

### Étape 3: Consentement
```
SSO affiche page /connect/authorize
    ↓
User clique "Autoriser"
    ↓
SSO génère authorization_code (43 caractères)
    ↓
Stocke dans AuthorizationCodeStore avec:
    - AccessToken (session)
    - ClientId
    - UserEmail
    ↓
Redirige vers: http://localhost:5173/callback?code=xxx
```

### Étape 4: Échange de code contre tokens
```
Client Backend envoie:
    POST /connect/token
    grant_type=authorization_code
    code=xxx
    client_id=gestion-personnel
    client_secret=secret-gestion-personnel-2024
    code_verifier=xxx
    ↓
SSO valide:
    ✓ Code existe et non expiré (5 min)
    ✓ Client ID correspond
    ✓ Code verifier (PKCE)
    ↓
SSO récupère User depuis UserEmail stocké
    ↓
SSO génère 2 tokens JWT:
    1. access_token (avec roles + permissions)
    2. id_token (avec infos utilisateur)
    ↓
Retourne:
    {
        "access_token": "eyJhbGc...",
        "id_token": "eyJhbGc...",
        "token_type": "Bearer",
        "expires_in": 3600
    }
```

## 🔑 STRUCTURE DES TOKENS JWT

### Access Token (pour APIs)
```json
{
  "sub": "65fe6e8b-1a2c-417a-b52c-cc6c8cf64ac5",
  "email": "admin@onee.ma",
  "jti": "unique-token-id",
  "iat": 1787584106,
  "role": ["Admin", "Manager"],
  "permission": [
    "users.read", "users.write", "users.delete",
    "roles.read", "roles.write",
    "dashboard.view"
  ],
  "exp": 1787587706,
  "iss": "ONEE.SSO",
  "aud": "https://localhost:5205"
}
```

### ID Token (pour identification - OIDC)
```json
{
  "sub": "65fe6e8b-1a2c-417a-b52c-cc6c8cf64ac5",
  "email": "admin@onee.ma",
  "name": "Admin User",
  "email_verified": "true",
  "jti": "unique-id-token-id",
  "iat": 1787584106,
  "exp": 1787587706,
  "iss": "ONEE.SSO",
  "aud": "gestion-personnel"
}
```

## 👥 DONNÉES SEED (Pré-remplies)

### Utilisateurs de test
| Email | Mot de passe | Rôles |
|-------|--------------|-------|
| admin@onee.ma | Admin@123 | Admin, Manager |
| chef.rh@onee.ma | ChefRH@123 | Manager |
| employe.1@onee.ma | Employe@123 | Employe |

### Applications clientes
| Nom | Client ID | Port Frontend | Port Backend |
|-----|-----------|---------------|--------------|
| Gestion Personnel | gestion-personnel | 5173 | 5291 |
| TIMS | tims-app | 5175 | 5115 |
| EAMS | eams-spa | 5173 | 5137 |

### Rôles et permissions
- **Admin**: Toutes les permissions (12)
- **Manager**: Lecture/Écriture (8)
- **Employe**: Lecture seule (4)

## 🎨 DESIGN SYSTEM ONEE

### Palette de couleurs
```css
--primary-blue: #1e3a8a    /* Bleu ONEE principal */
--primary-green: #10b981   /* Vert ONEE */
--primary-orange: #f59e0b  /* Orange ONEE */
--primary-dark: #0f172a
--text-muted: #64748b
```

### Composants
- Cards avec shadow et hover effects
- Buttons avec gradients
- Forms avec validation visuelle
- Stats cards avec icônes

## 📊 FONCTIONNALITÉS IMPLÉMENTÉES

### ✅ Phase 1: Core SSO
- [x] Login SSO avec email/password
- [x] Génération JWT avec roles + permissions
- [x] Page de consentement OAuth2
- [x] Authorization Code Flow (OIDC)
- [x] Token endpoint (/connect/token)
- [x] Logout endpoint (/connect/logout)
- [x] PKCE (Proof Key for Code Exchange)
- [x] Génération id_token (OIDC standard)
- [x] Intégration avec 3 applications clientes

### 🚧 Phase 2: Interface Admin SSO (En cours)
- [x] Dashboard SSO - Statistiques et aperçu
- [ ] Gestion des utilisateurs (CRUD)
- [ ] Gestion des rôles et permissions
- [ ] Gestion des applications clientes
- [ ] Sessions actives
- [ ] Logs d'audit

## 🐛 PROBLÈMES CONNUS

### 1. Token Validation dans RH Backend
**Symptôme**: Le backend RH rejette le JWT avec erreur `IDX10517: Signature validation failed. The token's kid is missing.`

**Cause**: Le JWT généré n'inclut pas le `kid` (Key ID) dans le header, requis par le validateur JWT.

**Solution à implémenter**:
```csharp
// Dans JwtService.cs, ajouter:
var header = new JwtHeader(credentials);
header.Add("kid", "onee-sso-key-2024");
```

### 2. Frontend RH - Redirection après callback
**Symptôme**: Le dashboard s'affiche 1 seconde puis retourne au login.

**Cause**: `oidc-client-ts` appelle automatiquement `/connect/logout` après le callback.

**Solution**: Désactiver `automaticSilentRenew` et `loadUserInfo` dans `authConfig.js`.

## 📁 COMMANDES UTILES

### Démarrer tout le système
```powershell
# Terminal 1 - SSO
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet run

# Terminal 2 - Backend RH
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\gestion-personnel\backend
dotnet run

# Terminal 3 - Frontend RH
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\gestion-personnel\frontend
npm run dev
```

### Accès aux applications
- **SSO Login**: https://localhost:5205/Login
- **SSO Dashboard**: https://localhost:5205/Dashboard
- **RH Frontend**: http://localhost:5173
- **RH Backend API**: http://localhost:5291/api

## 📈 STATISTIQUES DU PROJET

- **Durée développement**: 2 sprints
- **Lignes de code (SSO Backend)**: ~3000+
- **Technologies**: ASP.NET Core 9, React 19, SQL Server, JWT, OIDC
- **Pages Razor**: 6 (Login, Logout, Authorize, Dashboard, ForgotPassword, ResetPassword)
- **Endpoints API**: 3 (/connect/authorize, /connect/token, /connect/logout)
- **Entités Database**: 7 (User, Role, Permission, UserRole, RolePermission, ClientApplication, AuditLog)

## 🎓 POUR LE RAPPORT

### Points forts à mentionner
1. **Architecture Clean**: Séparation Domain/Application/Infrastructure
2. **Sécurité**: PKCE, JWT, Password hashing (BCrypt), CORS
3. **Standards**: Conformité OIDC/OAuth2
4. **Scalabilité**: Architecture modulaire, facilement extensible
5. **UX/UI**: Design professionnel avec couleurs ONEE
6. **Seed Data**: Base de données pré-remplie pour tests

### Diagrammes à inclure
1. Architecture globale (3 applications + SSO)
2. Flow d'authentification OIDC (séquence)
3. Structure base de données (ERD)
4. Architecture Clean (couches)

---

**Date de création**: Janvier 2025  
**Auteur**: [Ton nom]  
**Encadrant**: [Nom encadrant]  
**Entreprise**: ONEE (Office National de l'Électricité et de l'Eau potable)
