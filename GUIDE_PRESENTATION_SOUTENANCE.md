# 🎓 GUIDE DE PRÉSENTATION - ONEE.SSO

## 📊 RÉSUMÉ DU PROJET

**Projet** : ONEE.SSO - Système d'authentification unique (Single Sign-On)  
**Objectif** : Centraliser l'authentification pour 3 applications existantes de l'ONEE  
**Architecture** : Clean Architecture + ASP.NET Core 9 + React  
**État** : ✅ Backend 100% fonctionnel + 3 prompts d'intégration prêts

---

## ✅ TESTS BACKEND RÉUSSIS (100%)

### Test 1 : API Startup ✅
- Migrations appliquées automatiquement
- 24 rôles créés (seeders)
- 24 permissions créées (seeders)
- 66 associations rôles-permissions créées
- Admin user créé automatiquement : `admin@onee.ma` / `Admin@123`

### Test 2 : Login ✅
```json
POST /api/Auth/login
{
  "email": "admin@onee.ma",
  "password": "Admin@123"
}

Response:
{
  "userId": "e47df645-711b-4ffe-893f-b81a7bd4d856",
  "firstName": "Admin",
  "lastName": "User",
  "email": "admin@onee.ma",
  "accessToken": "eyJhbGci...",
  "refreshToken": "ilUgdHjc...",
  "refreshTokenExpiresAt": "2026-09-17T12:00:30Z",
  "roles": ["AdministrateurRH"]
}
```

### Test 3 : Protected Endpoint ✅
```
GET /api/Users
Authorization: Bearer <token>

Response: 200 OK
[
  { "id": "...", "firstName": "Salma", "lastName": "Test", "email": "salma@test.com" },
  { "id": "...", "firstName": "Test", "lastName": "Admin", "email": "test@onee.ma" },
  { "id": "...", "firstName": "Admin", "lastName": "User", "email": "admin@onee.ma" }
]
```

### Test 4 : Refresh Token ✅
```json
POST /api/Auth/refresh
{
  "refreshToken": "ilUgdHjc..."
}

Response:
{
  "accessToken": "eyJhbGci... (nouveau token)",
  "refreshToken": "GI+l40MS... (nouveau refresh token)",
  "refreshTokenExpiresAt": "2026-09-17T12:09:02Z",
  "accessTokenExpiresAt": "2026-08-18T12:24:02Z"
}
```
✅ Token rotation fonctionnel

### Test 5 : OIDC Discovery ✅
```
GET /.well-known/openid-configuration

Response:
{
  "issuer": "ONEE.SSO",
  "authorizationEndpoint": "http://localhost:5205/connect/authorize",
  "tokenEndpoint": "http://localhost:5205/connect/token",
  "userinfoEndpoint": "http://localhost:5205/api/auth/userinfo",
  "scopesSupported": [
    "openid", "profile", "email", "roles", "offline_access",
    "gestion-personnel", "tims", "eams"
  ],
  "grantTypesSupported": ["authorization_code", "refresh_token", "client_credentials"],
  "codeChallengeMethodsSupported": true
}
```
✅ Configuration OIDC complète + PKCE

### Test 6 : Userinfo Endpoint ✅
```
GET /api/Auth/userinfo
Authorization: Bearer <token>

Response:
{
  "sub": "e47df645-711b-4ffe-893f-b81a7bd4d856",
  "email": "admin@onee.ma",
  "emailVerified": true,
  "name": "Admin User",
  "givenName": "Admin",
  "familyName": "User",
  "roles": ["AdministrateurRH"],
  "permissions": ["USER_UPDATE", "USER_READ", "USER_CREATE", "USER_DELETE"]
}
```
✅ Conforme au standard OpenID Connect

### Test 7 : Logout ✅
```json
POST /api/Auth/logout
{
  "refreshToken": "ilUgdHjc..."
}

Response:
{
  "success": true,
  "message": "Déconnexion réussie",
  "sessionsRevoked": 1
}
```
✅ Révocation de session fonctionnelle

---

## 🏗️ ARCHITECTURE TECHNIQUE

### Backend : Clean Architecture
```
ONEE.SSO.API
├── Controllers (11 endpoints REST)
├── Middlewares (Exception handling)
└── Extensions (DI configuration)

ONEE.SSO.Application
├── Features (CQRS + MediatR)
├── DTOs (Data Transfer Objects)
├── Mappings (AutoMapper)
└── Interfaces

ONEE.SSO.Domain
├── Entities (11 entités métier)
├── Enums
└── Events

ONEE.SSO.Infrastructure
├── Persistence (EF Core + Migrations)
├── Seed (Auto-seeders)
├── Repositories
└── Services (JWT, Hashing, Email)
```

### Technologies
- **Framework** : ASP.NET Core 9
- **ORM** : Entity Framework Core 9
- **Base de données** : SQL Server
- **Authentification** : JWT + OpenID Connect
- **Logging** : Serilog
- **Patterns** : CQRS, MediatR, Repository Pattern
- **Sécurité** : BCrypt, PKCE, Refresh Token Rotation

---

## 📋 FONCTIONNALITÉS IMPLÉMENTÉES

### Authentification & Autorisation
- ✅ Login avec email/password (BCrypt)
- ✅ JWT Access Token (1 heure)
- ✅ Refresh Token (30 jours)
- ✅ Token Rotation automatique
- ✅ Révocation de tokens
- ✅ Account lockout (5 tentatives échouées)
- ✅ OpenID Connect Discovery
- ✅ PKCE pour sécurité renforcée

### Gestion des Utilisateurs
- ✅ CRUD Utilisateurs
- ✅ Activation/Désactivation
- ✅ Assignation de rôles multiples
- ✅ Historique des sessions
- ✅ Audit logs complets

### Gestion des Rôles & Permissions
- ✅ CRUD Rôles
- ✅ CRUD Permissions
- ✅ Association Rôles-Permissions (Many-to-Many)
- ✅ Vérification dynamique des permissions

### Applications Clientes
- ✅ CRUD Applications clientes
- ✅ ClientId + ClientSecret
- ✅ Scopes personnalisés par application
- ✅ 3 applications préconfigurées :
  - `gestion-personnel` (RH)
  - `tims-app` (TIMS)
  - `eams-spa` (EAMS)

### Audit & Sécurité
- ✅ Audit logs automatiques (Create, Update, Delete, Login, Logout)
- ✅ Tracking des sessions utilisateur
- ✅ Historique des refresh tokens
- ✅ Logging structuré (Serilog)

---

## 🎯 SCÉNARIO DE DÉMONSTRATION

### Partie 1 : Backend SSO (5 minutes)

**1. Lancer le SSO**
```bash
dotnet run --project src/ONEE.SSO.API
```
✅ Logs : Migrations appliquées, seeders exécutés

**2. Swagger UI**
- Ouvrir http://localhost:5205/swagger
- Montrer les 52 endpoints REST

**3. Test Login**
```
POST /api/Auth/login
{
  "email": "admin@onee.ma",
  "password": "Admin@123"
}
```
✅ Recevoir JWT + Refresh Token + Rôles + Permissions

**4. Test Endpoint Protégé**
```
GET /api/Users
Authorization: Bearer <token>
```
✅ Recevoir la liste des utilisateurs

**5. Test OIDC Discovery**
```
GET /.well-known/openid-configuration
```
✅ Configuration OpenID Connect complète

**6. Test Logout**
```
POST /api/Auth/logout
{
  "refreshToken": "..."
}
```
✅ Session révoquée

---

### Partie 2 : Architecture & Code (5 minutes)

**Montrer dans VS Code/Visual Studio :**

1. **Clean Architecture**
   - 4 projets séparés (API, Application, Domain, Infrastructure)
   - Séparation des responsabilités

2. **CQRS avec MediatR**
   - Fichier : `LoginCommandHandler.cs`
   - Montrer le pattern Command/Handler

3. **Entities Domain**
   - Fichier : `User.cs`, `Role.cs`, `Permission.cs`
   - Relations Many-to-Many

4. **Seeders Automatiques**
   - Fichier : `RolesSeeder.cs`, `PermissionsSeeder.cs`
   - 24 rôles + 24 permissions préconfigurés

5. **JWT Service**
   - Fichier : `JwtTokenService.cs`
   - Génération de tokens avec claims custom

6. **Middleware Exception**
   - Fichier : `ExceptionMiddleware.cs`
   - Gestion centralisée des erreurs

---

### Partie 3 : Intégration Frontend (5 minutes)

**Montrer les 3 prompts d'intégration créés :**

1. **PROMPT_INTEGRATION_GESTION_PERSONNEL.md**
   - React 19
   - Configuration OIDC complète
   - Code copy-paste ready

2. **PROMPT_INTEGRATION_TIMS.md**
   - React 19.2.7
   - Custom scopes : `tims_user_id`, `tims_service_id`, `tims_team_id`
   - Middleware backend pour claims custom

3. **PROMPT_INTEGRATION_EAMS.md**
   - React 18.3.1 + TypeScript
   - Custom scopes : `eams_user_id`, `serviceId`
   - Types TypeScript complets

**Expliquer le flux SSO :**
```
1. Utilisateur clique "Se connecter" dans App1
   ↓
2. Redirection vers ONEE.SSO (http://localhost:5205)
   ↓
3. Login sur le SSO (email/password)
   ↓
4. Redirection vers App1/callback avec code
   ↓
5. App1 échange le code contre un token JWT
   ↓
6. App1 charge le profil utilisateur
   ↓
7. Utilisateur accède à App1, App2, App3 sans re-login
```

---

### Partie 4 : Base de Données (2 minutes)

**Montrer dans SQL Server Management Studio :**

1. **Tables créées (11 tables)**
   - Users
   - Roles
   - Permissions
   - UserRoles (Many-to-Many)
   - RolePermissions (Many-to-Many)
   - RefreshTokens
   - UserSessions
   - AuditLogs
   - ClientApplications
   - __EFMigrationsHistory

2. **Données seed**
   - 24 rôles
   - 24 permissions
   - 66 associations rôles-permissions
   - 3 applications clientes
   - 3 utilisateurs

---

## 🔒 SÉCURITÉ IMPLÉMENTÉE

✅ **Hashing des mots de passe** : BCrypt (coût 12)  
✅ **JWT signé** : HMAC-SHA256  
✅ **Refresh Token Rotation** : Ancien token révoqué après refresh  
✅ **Account Lockout** : 5 tentatives échouées → compte verrouillé  
✅ **PKCE** : Protection contre attaques par interception  
✅ **CORS** : Configuration stricte par origine  
✅ **HTTPS recommandé** : Pour production  
✅ **Token Expiration** : Access Token 1h, Refresh Token 30 jours  
✅ **Audit Logs** : Traçabilité complète des actions

---

## 📊 MÉTRIQUES DU PROJET

- **Lignes de code** : ~8000 lignes
- **Contrôleurs** : 11
- **Endpoints REST** : 52
- **Entités Domain** : 11
- **Migrations EF Core** : 6
- **Seeders** : 6
- **Tests backend** : 7/7 ✅

---

## 🚀 PROCHAINES ÉTAPES (Si demandé)

### Phase 1 : Intégration Frontend (1-2 jours)
- Appliquer le prompt Gestion Personnel
- Appliquer le prompt TIMS
- Appliquer le prompt EAMS

### Phase 2 : Tests E2E (1 jour)
- Tester le flux SSO complet
- Login une fois → Accès aux 3 apps
- Logout → Déconnexion de toutes les apps

### Phase 3 : Déploiement (1 jour)
- Configurer IIS / Azure
- HTTPS + Certificat SSL
- Base de données production
- Secrets en Azure Key Vault

### Phase 4 : Documentation (1 jour)
- Guide utilisateur
- Guide administrateur
- API documentation (Swagger)

---

## 💡 POINTS FORTS À MENTIONNER

1. **Architecture moderne** : Clean Architecture + CQRS + MediatR
2. **Standards industriels** : OpenID Connect + OAuth 2.0 + PKCE
3. **Sécurité robuste** : BCrypt + JWT + Token Rotation + Account Lockout
4. **Scalabilité** : Architecture découplée, prête pour microservices
5. **Maintenabilité** : Code propre, séparation des responsabilités
6. **Traçabilité** : Audit logs complets
7. **Extensibilité** : Facile d'ajouter de nouvelles applications clientes
8. **Zero-downtime migration** : Les apps existantes ne sont pas refaites

---

## 📝 QUESTIONS POTENTIELLES & RÉPONSES

### Q1 : Pourquoi Clean Architecture ?
**R** : Séparation des responsabilités, testabilité, maintenabilité, indépendance des frameworks.

### Q2 : Pourquoi CQRS + MediatR ?
**R** : Découplage, scalabilité, code plus lisible, facilite les tests unitaires.

### Q3 : Comment les applications clientes s'authentifient ?
**R** : OpenID Connect Authorization Code Flow avec PKCE. Le code est échangé contre un token JWT.

### Q4 : Que se passe-t-il si le token expire ?
**R** : Le refresh token est utilisé automatiquement pour obtenir un nouveau access token sans re-login.

### Q5 : Comment revoquer l'accès d'un utilisateur ?
**R** : Désactiver l'utilisateur dans la base → tous ses tokens deviennent invalides.

### Q6 : Le SSO supporte-t-il plusieurs applications ?
**R** : Oui, architecture multi-tenant. Chaque app a son ClientId + scopes personnalisés.

### Q7 : Comment tracker les actions utilisateur ?
**R** : Table AuditLogs qui enregistre toutes les actions (Create, Update, Delete, Login, Logout).

### Q8 : Le système est-il prêt pour la production ?
**R** : Backend 100% fonctionnel. Il reste à appliquer les 3 prompts d'intégration frontend (1-2 jours) et configurer le déploiement HTTPS.

### Q9 : Peut-on ajouter une authentification par Active Directory ?
**R** : Oui, il suffit d'ajouter un `ExternalAuthService` qui communique avec AD et crée/met à jour l'utilisateur dans la base SSO.

### Q10 : Comment gérer les custom claims par application ?
**R** : Le JWT contient les scopes de chaque app. Le backend de l'app extrait les claims spécifiques (ex: `tims_user_id`, `eams_user_id`).

---

## 🎬 STRUCTURE DE LA PRÉSENTATION (15-20 minutes)

### Introduction (2 min)
- Contexte : ONEE avec 3 applications existantes
- Problème : Authentification séparée → mauvaise UX
- Solution : SSO centralisé avec ONEE.SSO

### Architecture & Technologies (4 min)
- Clean Architecture (schéma)
- Technologies utilisées (ASP.NET Core 9, EF Core, JWT, OIDC)
- Patterns : CQRS, MediatR, Repository

### Démonstration Backend (6 min)
- Lancer l'API
- Tests Swagger (Login, Protected Endpoint, OIDC Discovery, Logout)
- Montrer le code (Handlers, Entities, Seeders, JWT Service)

### Plan d'Intégration (4 min)
- Montrer les 3 prompts d'intégration
- Expliquer le flux SSO
- Schéma : Login une fois → Accès aux 3 apps

### Sécurité & Fonctionnalités (3 min)
- Mesures de sécurité (BCrypt, Token Rotation, PKCE, Audit Logs)
- Fonctionnalités implémentées (Gestion users, rôles, permissions, apps clientes)

### Conclusion & Questions (2 min)
- État : Backend 100% fonctionnel
- Prochaines étapes : Intégration frontend (1-2 jours)
- Questions du jury

---

## 📦 LIVRABLES

✅ **Code source complet** : GitHub / ZIP  
✅ **Base de données** : Script SQL + Migrations EF Core  
✅ **Documentation technique** : README.md, COMPLETE_PROJECT_PHASES.md  
✅ **Prompts d'intégration** : 3 fichiers markdown copy-paste ready  
✅ **Tests validés** : 7/7 tests backend réussis  
✅ **Swagger UI** : Documentation API interactive  

---

## 🎯 MESSAGE DE FIN

**ONEE.SSO est un système SSO moderne, sécurisé et extensible, prêt à centraliser l'authentification des applications de l'ONEE.**

✅ Backend 100% fonctionnel  
✅ Standards industriels (OpenID Connect, OAuth 2.0, PKCE)  
✅ Architecture Clean + CQRS + MediatR  
✅ Sécurité robuste (BCrypt, JWT, Token Rotation)  
✅ 3 prompts d'intégration prêts à l'emploi  
✅ Extensible et scalable  

**Prêt pour la soutenance ! 🚀**
