# 📂 ARBORESCENCE DES 3 APPLICATIONS INTÉGRÉES SSO

## 📋 SOMMAIRE

1. [Gestion Personnel (RH)](#1-gestion-personnel-rh)
2. [TIMS (Gestion des Interventions)](#2-tims-gestion-des-interventions)
3. [EAMS (Gestion des Équipements)](#3-eams-gestion-des-équipements)

---

## 1️⃣ GESTION PERSONNEL (RH)

### 📍 Chemin de base
```
c:\Users\XPS\Desktop\Gestion du Prsonnel\
```

### 📂 Structure complète

```
Gestion du Prsonnel/
├── backend/
│   └── GestionPersonnel.API/
│       ├── appsettings.json                   ⚙️ Config JWT SSO
│       ├── Program.cs                         ⚙️ Config JWT Bearer
│       ├── Controllers/
│       │   ├── EmployesController.cs          📝 API Employés
│       │   ├── AuthController.cs              🔐 (si existe)
│       │   └── ...
│       └── ...
│
└── frontend/
    ├── package.json                           📦 Dépendances (oidc-client-ts)
    ├── src/
    │   ├── config/
    │   │   └── authConfig.js                  ⚙️ Config OIDC SSO
    │   ├── services/
    │   │   └── authService.js                 🔐 Service SSO
    │   ├── pages/
    │   │   ├── Login.jsx                      🖥️ Page Login SSO
    │   │   ├── Callback.jsx                   🔄 Callback SSO
    │   │   └── ...
    │   ├── components/
    │   │   ├── ProtectedRoute.jsx             🛡️ Route protégée
    │   │   └── ...
    │   ├── context/
    │   │   └── AuthContext.jsx                📦 Context SSO
    │   └── utils/
    │       └── axiosInstance.js               🌐 Axios + token
    │
    └── public/
        └── silent-renew.html                  🔄 Renouvellement token
```

### 🚀 Commandes de lancement

**Backend (Port 5291)**
```powershell
cd "c:\Users\XPS\Desktop\Gestion du Prsonnel\backend\GestionPersonnel.API"
dotnet run
```

**Frontend (Port 5173)**
```powershell
cd "c:\Users\XPS\Desktop\Gestion du Prsonnel\frontend"
npm run dev
```

### 📋 Fichiers clés SSO

| Fichier | Description |
|---------|-------------|
| `backend/appsettings.json` | Configuration JWT (Issuer, Audience, SecretKey) |
| `backend/Program.cs` | Configuration Authentication JWT Bearer |
| `frontend/src/config/authConfig.js` | Configuration OIDC (SSO URL, ClientId, Scopes) |
| `frontend/src/services/authService.js` | Méthodes login/logout/getUser |
| `frontend/src/pages/Callback.jsx` | Traitement callback après auth SSO |

### 📄 Documentation

```
Gestion du Prsonnel/
├── README_SSO.md                              📖 Guide complet SSO
├── DEMARRAGE_RAPIDE.md                        🚀 Guide démarrage 3 étapes
├── LISEZ-MOI-EN-PREMIER.txt                   ⭐ Résumé ultra-concis
├── RESUME_FINAL_SSO.md                        📊 Résumé détaillé
├── SSO_INTEGRATION_COMPLETE.md                📚 Documentation technique
├── INTEGRATION_SSO_GUIDE.md                   📘 Guide d'intégration
├── INDEX_DOCUMENTATION.md                     📑 Index documentation
├── START_ALL_SERVERS.ps1                      🚀 Script de lancement
├── VERIFY_SSO_SETUP.ps1                       ✅ Script de vérification
└── TEST_SSO_INTEGRATION.ps1                   🧪 Script de test
```

---

## 2️⃣ TIMS (GESTION DES INTERVENTIONS)

### 📍 Chemin de base
```
c:\Users\XPS\Desktop\gestion des interventions\
```

### 📂 Structure complète

```
gestion des interventions/
├── backend/
│   └── TIMS.API/
│       ├── appsettings.json                   ⚙️ Config JWT SSO
│       ├── appsettings.Sso.json               ⚙️ Config SSO séparée
│       ├── Program.cs                         ⚙️ Config JWT + Middleware
│       ├── Middlewares/
│       │   └── TimsContextMiddleware.cs       🔧 Extraction custom claims TIMS
│       ├── Controllers/
│       │   ├── InterventionsController.cs     📝 API Interventions
│       │   ├── SsoTestController.cs           🧪 3 endpoints test SSO
│       │   └── ...
│       └── ...
│
└── frontend/
    ├── package.json                           📦 Dépendances (oidc-client-ts)
    ├── src/
    │   ├── config/
    │   │   └── authConfig.js                  ⚙️ Config OIDC + scopes TIMS
    │   ├── services/
    │   │   └── authService.js                 🔐 Service SSO + claims TIMS
    │   ├── pages/
    │   │   ├── LoginSSO.jsx                   🖥️ Page Login SSO
    │   │   ├── Callback.jsx                   🔄 Callback SSO
    │   │   ├── DashboardSSO.jsx               🖥️ Dashboard test SSO
    │   │   └── ...
    │   ├── components/
    │   │   ├── ProtectedRoute.jsx             🛡️ Route protégée
    │   │   ├── SsoUserMenu.jsx                👤 Menu utilisateur SSO
    │   │   └── ...
    │   ├── context/
    │   │   └── AuthContextSSO.jsx             📦 Context SSO TIMS
    │   ├── hooks/
    │   │   └── useSsoAuth.js                  🪝 Hook custom SSO
    │   └── utils/
    │       ├── axiosInstanceSSO.js            🌐 Axios + token + headers TIMS
    │       └── ssoHelpers.js                  🛠️ Helpers SSO
    │
    └── public/
        └── silent-renew.html                  🔄 Renouvellement token
```

### 🚀 Commandes de lancement

**Backend (Port 5115)**
```powershell
cd "c:\Users\XPS\Desktop\gestion des interventions\backend\TIMS.API"
dotnet run
```

**Frontend (Port auto)**
```powershell
cd "c:\Users\XPS\Desktop\gestion des interventions\frontend"
npm run dev
```

### 📋 Fichiers clés SSO + Custom Claims TIMS

| Fichier | Description |
|---------|-------------|
| `backend/appsettings.Sso.json` | Configuration JWT SSO séparée |
| `backend/Middlewares/TimsContextMiddleware.cs` | **Extraction custom claims TIMS** (tims_user_id, tims_service_id, tims_team_id) |
| `backend/Program.cs` | Configuration JWT + Middleware custom |
| `backend/Controllers/SsoTestController.cs` | **3 endpoints de test** (verify-claims, user-profile, protected) |
| `frontend/src/config/authConfig.js` | Config OIDC + **scopes TIMS** (tims_user_id, tims_service_id, tims_team_id) |
| `frontend/src/services/authService.js` | Méthodes SSO + **extraction custom claims TIMS** |
| `frontend/src/utils/axiosInstanceSSO.js` | Interceptor Axios + **headers custom TIMS** (X-TIMS-User-Id, X-TIMS-Service-Id, X-TIMS-Team-Id) |

### 🎯 Custom Claims TIMS

```javascript
{
  "tims_user_id": "...",      // ID utilisateur dans TIMS
  "tims_service_id": "...",   // ID du service
  "tims_team_id": "..."       // ID de l'équipe/EquipeId
}
```

### 🌐 Custom Headers HTTP TIMS

```
X-TIMS-User-Id: [valeur]
X-TIMS-Service-Id: [valeur]
X-TIMS-Team-Id: [valeur]
```

### 📄 Documentation

```
gestion des interventions/
├── README_SSO_INTEGRATION.md                  📖 Documentation principale ⭐
├── TEST_SSO_GUIDE.md                          🧪 Tests étape par étape
├── MIGRATION_SSO_GUIDE.md                     📋 Plan de migration complet
├── COMMANDS_SSO.md                            ⚡ Commandes rapides
├── SUMMARY_INTEGRATION_SSO.md                 📊 Résumé intégration
└── START_SSO_TIMS.ps1                         🚀 Script de lancement auto
```

---

## 3️⃣ EAMS (GESTION DES ÉQUIPEMENTS)

### 📍 Chemin de base
```
c:\Users\XPS\Desktop\gestion des equipements\
```

### 📂 Structure complète

```
gestion des equipements/
├── backend/
│   └── ONEE.EAMS.API/
│       ├── appsettings.json                   ⚙️ Config JWT SSO
│       ├── Program.cs                         ⚙️ Config JWT + Middleware
│       ├── Middlewares/
│       │   └── EamsContextMiddleware.cs       ⚙️ Extraction custom claims EAMS
│       ├── Controllers/
│       │   ├── EquipementsController.cs       📝 API Équipements (avec claims)
│       │   ├── SsoTestController.cs           🧪 3 endpoints test SSO
│       │   └── ...
│       └── ...
│
└── frontend/
    ├── package.json                           📦 Dépendances (oidc-client-ts, @types/node)
    ├── tsconfig.json                          ⚙️ Config TypeScript
    ├── src/
    │   ├── types/
    │   │   └── types.ts                       📝 Types TS (UserProfile, EamsContext, AuthState)
    │   ├── config/
    │   │   └── authConfig.ts                  ⚙️ Config OIDC + scopes EAMS (TypeScript)
    │   ├── services/
    │   │   └── authService.ts                 🔐 Service SSO + claims EAMS (TypeScript)
    │   ├── pages/
    │   │   ├── LoginSSO.tsx                   🖥️ Page Login SSO (TypeScript)
    │   │   ├── Callback.tsx                   🔄 Callback SSO (TypeScript)
    │   │   └── ...
    │   ├── components/
    │   │   ├── ProtectedRouteSSO.tsx          🛡️ Route protégée + rôles (TypeScript)
    │   │   └── ...
    │   └── utils/
    │       └── axiosInstanceSSO.ts            🌐 Axios + token + headers EAMS (TypeScript)
    │
    └── public/
        └── silent-renew.html                  🔄 Renouvellement token
```

### 🚀 Commandes de lancement

**Backend (Port 5137)**
```powershell
cd "c:\Users\XPS\Desktop\gestion des equipements\backend\ONEE.EAMS.API"
dotnet run
```

**Frontend (Port auto)**
```powershell
cd "c:\Users\XPS\Desktop\gestion des equipements\frontend"
npm run dev
```

### 📋 Fichiers clés SSO + Custom Claims EAMS

| Fichier | Description |
|---------|-------------|
| `backend/appsettings.json` | Configuration JWT SSO |
| `backend/Middlewares/EamsContextMiddleware.cs` | **Extraction custom claims EAMS** (eams_user_id, serviceId) |
| `backend/Program.cs` | Configuration JWT + Middleware custom |
| `backend/Controllers/SsoTestController.cs` | **3 endpoints de test** (profile, equipments, admin-only) |
| `backend/Controllers/EquipementsController.cs` | Exemple d'utilisation custom claims pour filtrage RBAC |
| `frontend/src/types/types.ts` | **Types TypeScript** (UserProfile, EamsContext, AuthState) |
| `frontend/src/config/authConfig.ts` | Config OIDC + **scopes EAMS** (eams_user_id, serviceId) |
| `frontend/src/services/authService.ts` | Méthodes SSO + **extraction custom claims EAMS** |
| `frontend/src/utils/axiosInstanceSSO.ts` | Interceptor Axios + **headers custom EAMS** (X-EAMS-User-Id, X-EAMS-Service-Id) |

### 🎯 Custom Claims EAMS

```typescript
{
  "eams_user_id": "...",   // ID utilisateur dans EAMS (pour base locale)
  "serviceId": "..."       // ID du service pour filtrage RBAC
}
```

### 🌐 Custom Headers HTTP EAMS

```
X-EAMS-User-Id: [valeur]
X-EAMS-Service-Id: [valeur]
```

### 📄 Documentation

```
gestion des equipements/
└── SSO_INTEGRATION_SUMMARY.md                 📖 Documentation complète
```

---

## 🔐 SERVEUR SSO CENTRAL

### 📍 Chemin de base
```
c:\Users\XPS\source\repos\ONEE.SSO\
```

### 📂 Structure simplifiée

```
ONEE.SSO/
├── src/
│   └── ONEE.SSO.API/
│       ├── appsettings.json                   ⚙️ Config JWT (SecretKey ici)
│       ├── Program.cs                         ⚙️ Config serveur SSO
│       ├── Controllers/
│       │   ├── AuthController.cs              🔐 Login, Refresh, Logout, UserInfo
│       │   ├── UsersController.cs             👤 Gestion utilisateurs
│       │   ├── RolesController.cs             🎭 Gestion rôles
│       │   ├── PermissionsController.cs       🔑 Gestion permissions
│       │   └── ...
│       ├── Services/
│       │   ├── TokenService.cs                🎟️ Génération JWT
│       │   └── ...
│       └── Logs/
│           └── log-YYYYMMDD.txt               📋 Logs quotidiens
│
├── GUIDE_TEST_COMPLET_3_APPLICATIONS.md       📖 ⭐ Guide de test complet
├── LANCER_TOUS_LES_SERVEURS.ps1               🚀 Script lancement auto
├── COMMANDES_MANUELLES.md                     📋 Commandes manuelles
├── GUIDE_TESTS_E2E.md                         🧪 Tests E2E complets
├── RAPPORT_VERIFICATION_FINAL.md              ✅ Rapport d'intégration 100%
└── ...
```

### 🚀 Commande de lancement

**Serveur SSO (Port 5205)**
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet run
```

---

## 📊 RÉSUMÉ DES PORTS

| Service | Port | URL |
|---------|------|-----|
| 🔐 **SSO** | 5205 | http://localhost:5205 |
| 📊 **Backend RH** | 5291 | http://localhost:5291 |
| 🔧 **Backend TIMS** | 5115 | http://localhost:5115 |
| ⚙️ **Backend EAMS** | 5137 | http://localhost:5137 |
| 🖥️ **Frontend RH** | 5173 | http://localhost:5173 |
| 🖥️ **Frontend TIMS** | Auto | Vite choisira automatiquement |
| 🖥️ **Frontend EAMS** | Auto | Vite choisira automatiquement |

---

## 🔑 IDENTIFIANTS DE TEST

```
Email    : admin@onee.ma
Password : Admin@123
```

---

## 🎯 FICHIERS CLÉS PAR APPLICATION

### Gestion Personnel
- Backend : `appsettings.json`, `Program.cs`
- Frontend : `authConfig.js`, `authService.js`, `Callback.jsx`

### TIMS (avec custom claims)
- Backend : `appsettings.Sso.json`, `TimsContextMiddleware.cs`, `SsoTestController.cs`
- Frontend : `authConfig.js` (scopes TIMS), `axiosInstanceSSO.js` (headers custom)

### EAMS (TypeScript + custom claims)
- Backend : `EamsContextMiddleware.cs`, `SsoTestController.cs`, `EquipementsController.cs`
- Frontend : `types.ts`, `authConfig.ts`, `authService.ts`, `axiosInstanceSSO.ts`

---

## 🚀 LANCEMENT RAPIDE

### Option 1 : Script automatique (RECOMMANDÉ)
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO
.\LANCER_TOUS_LES_SERVEURS.ps1
```

### Option 2 : Commandes manuelles
Voir le fichier : `COMMANDES_MANUELLES.md`

---

## 📖 GUIDE DE TEST

Pour tester les 3 applications, suivre le guide :
```
GUIDE_TEST_COMPLET_3_APPLICATIONS.md
```

---

**🎉 SYSTÈME SSO COMPLET ET OPÉRATIONNEL !**
