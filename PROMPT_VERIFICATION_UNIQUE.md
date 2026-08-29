# 🔍 VÉRIFICATION INTÉGRATION SSO - PROMPT UNIVERSEL

## 🎯 INSTRUCTION

Je viens d'intégrer cette application avec le serveur SSO ONEE (http://localhost:5205). **Vérifie que TOUT est correctement configuré** et génère un rapport complet.

---

## 📋 CE QUE TU DOIS FAIRE

### ÉTAPE 1 : IDENTIFIER L'APPLICATION

Regarde le dossier actuel et identifie quelle application tu vérifies :

- Si le dossier contient **"Gestion du Prsonnel"** ou **"personnel"** → **Application RH**
  - ClientId: `gestion-personnel`
  - Port Backend: 5291
  - Custom Claims: Aucun

- Si le dossier contient **"gestion des interventions"** ou **"TIMS"** → **Application TIMS**
  - ClientId: `tims-app`
  - Port Backend: 5115
  - Custom Claims: `tims_user_id`, `tims_service_id`, `tims_team_id`

- Si le dossier contient **"gestion des equipements"** ou **"EAMS"** → **Application EAMS**
  - ClientId: `eams-spa`
  - Port Backend: 5137
  - Custom Claims: `eams_user_id`, `serviceId`
  - TypeScript: Oui

---

### ÉTAPE 2 : VÉRIFIER LE FRONTEND

**Cherche ces fichiers** (cherche dans `src/`, `frontend/src/`, ou le dossier source React) :

#### Fichiers SSO obligatoires :
- [ ] `authConfig.js` (ou `.ts` pour EAMS)
- [ ] `authService.js` (ou `.ts` pour EAMS)
- [ ] Page de login SSO (peut s'appeler `Login.jsx`, `LoginSSO.jsx`, `LoginSSO.tsx`)
- [ ] `Callback.jsx` (ou `.tsx` pour EAMS)
- [ ] `ProtectedRoute.jsx` (ou `ProtectedRouteSSO.jsx/tsx`)
- [ ] Configuration axios (peut s'appeler `axiosConfig.js`, `axiosInstance.js`, `axiosInstanceSSO.ts`)
- [ ] `public/silent-renew.html`

#### Vérifications dans authConfig :
```javascript
// Vérifie que ces valeurs existent et sont correctes :
- CLIENT_ID = correct selon l'application identifiée
- AUTHORITY = 'http://localhost:5205'
- REDIRECT_URI = 'http://localhost:5173/callback'
- scope contient au minimum : 'openid profile email roles offline_access'
- Pour TIMS : scope contient aussi 'tims tims_user_id tims_service_id tims_team_id'
- Pour EAMS : scope contient aussi 'eams eams_user_id serviceId'
```

#### Vérifications dans authService :
- [ ] Méthode `login()` existe
- [ ] Méthode `completeLogin()` existe
- [ ] Méthode `logout()` existe
- [ ] Méthode `getAccessToken()` existe
- [ ] Méthode `getUserProfile()` existe
- [ ] Pour TIMS : Méthodes `getTimsUserId()`, `getTimsServiceId()`, `getTimsTeamId()` existent
- [ ] Pour EAMS : Méthodes `getEamsUserId()`, `getServiceId()` existent

#### Vérifications dans axios :
- [ ] Interceptor ajoute le token : `Authorization: Bearer ${token}`
- [ ] Pour TIMS : Ajoute headers `X-TIMS-User-Id`, `X-TIMS-Service-Id`, `X-TIMS-Team-Id`
- [ ] Pour EAMS : Ajoute headers `X-EAMS-User-Id`, `X-EAMS-Service-Id`

#### Vérifications package.json :
- [ ] Dépendance `oidc-client-ts` installée
- [ ] Dépendance `react-router-dom` installée
- [ ] Pour EAMS : Dépendance `@types/node` installée

#### Vérifications TypeScript (EAMS uniquement) :
- [ ] Fichier `types.ts` existe avec interfaces `UserProfile`, `EamsContext`
- [ ] Tous les fichiers SSO sont en `.ts` ou `.tsx` (pas `.js`)

---

### ÉTAPE 3 : VÉRIFIER LE BACKEND

**Cherche ces fichiers** (cherche dans `backend/`, `API/`, ou le dossier .NET) :

#### Fichiers obligatoires :
- [ ] `appsettings.json` contient section `JwtSettings`
- [ ] `Program.cs` configure JWT Authentication
- [ ] Pour TIMS : `Middlewares/TimsContextMiddleware.cs` existe
- [ ] Pour EAMS : `Middlewares/EamsContextMiddleware.cs` existe

#### Vérifications dans appsettings.json :
```json
"JwtSettings": {
  "Issuer": "ONEE.SSO",  // ✅ Doit être exactement ça
  "Audience": "ONEE.Applications",  // ✅ Doit être exactement ça
  "SecretKey": "..." // ✅ Doit exister (32+ caractères)
}
```

#### Vérifications dans Program.cs :
- [ ] `builder.Services.AddAuthentication()` avec `JwtBearerDefaults.AuthenticationScheme`
- [ ] `TokenValidationParameters` valide `Issuer`, `Audience`, `IssuerSigningKey`
- [ ] `IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))`
- [ ] CORS autorise `http://localhost:5173`
- [ ] `app.UseAuthentication()` appelé
- [ ] `app.UseAuthorization()` appelé
- [ ] Pour TIMS : `app.UseTimsContext()` appelé APRÈS `UseAuthentication()`
- [ ] Pour EAMS : `app.UseEamsContext()` appelé APRÈS `UseAuthentication()`

#### Vérifications Middleware Custom (TIMS et EAMS) :
**TIMS (TimsContextMiddleware.cs) :**
```csharp
// Doit extraire ces claims du JWT :
var timsUserId = context.User.FindFirst("tims_user_id")?.Value;
var timsServiceId = context.User.FindFirst("tims_service_id")?.Value;
var timsTeamId = context.User.FindFirst("tims_team_id")?.Value;

// Et les ajouter dans HttpContext.Items :
context.Items["TimsUserId"] = timsUserId;
context.Items["TimsServiceId"] = timsServiceId;
context.Items["TimsTeamId"] = timsTeamId;
```

**EAMS (EamsContextMiddleware.cs) :**
```csharp
// Doit extraire ces claims du JWT :
var eamsUserId = context.User.FindFirst("eams_user_id")?.Value;
var serviceId = context.User.FindFirst("serviceId")?.Value;

// Et les ajouter dans HttpContext.Items :
context.Items["EamsUserId"] = eamsUserId;
context.Items["ServiceId"] = serviceId;
```

#### Vérifications Controllers :
- [ ] Au moins un controller a l'attribut `[Authorize]`
- [ ] Les controllers peuvent accéder aux claims : `User.FindFirst("sub")?.Value`

---

### ÉTAPE 4 : TESTER LA COMPILATION

**Backend :**
```bash
dotnet build --no-restore
```
- [ ] Compilation réussit sans erreur
- [ ] Pas de warning critique sur JWT ou Authentication

**Frontend (EAMS uniquement) :**
```bash
npm run build
```
- [ ] TypeScript compile sans erreur sur les fichiers SSO

---

### ÉTAPE 5 : GÉNÉRER LE RAPPORT

Génère un rapport dans ce format :

```
═══════════════════════════════════════════════════════════
🔍 VÉRIFICATION SSO - [NOM DE L'APPLICATION DÉTECTÉE]
═══════════════════════════════════════════════════════════

📋 IDENTIFICATION
Application : [Gestion Personnel / TIMS / EAMS]
ClientId : [gestion-personnel / tims-app / eams-spa]
Port Backend : [5291 / 5115 / 5137]
Custom Claims : [Aucun / TIMS: 3 / EAMS: 2]
TypeScript : [Oui / Non]

═══════════════════════════════════════════════════════════
📁 FRONTEND
═══════════════════════════════════════════════════════════

✅ Fichiers SSO
[✅/❌] authConfig.[js/ts] trouvé
[✅/❌] authService.[js/ts] trouvé
[✅/❌] Page Login SSO trouvée
[✅/❌] Callback.[jsx/tsx] trouvé
[✅/❌] ProtectedRoute trouvé
[✅/❌] Axios config trouvé
[✅/❌] silent-renew.html trouvé

✅ Configuration OIDC
[✅/❌] CLIENT_ID correct : [valeur trouvée]
[✅/❌] AUTHORITY : http://localhost:5205
[✅/❌] REDIRECT_URI : http://localhost:5173/callback
[✅/❌] Scopes de base présents (openid, profile, email, roles, offline_access)
[✅/❌] Scopes custom présents (si TIMS ou EAMS)

✅ AuthService
[✅/❌] Méthode login() présente
[✅/❌] Méthode completeLogin() présente
[✅/❌] Méthode logout() présente
[✅/❌] Méthode getAccessToken() présente
[✅/❌] Méthodes custom présentes (si TIMS ou EAMS)

✅ Axios Interceptor
[✅/❌] Ajoute Bearer token
[✅/❌] Ajoute headers custom (si TIMS ou EAMS)

✅ Packages
[✅/❌] oidc-client-ts installé
[✅/❌] react-router-dom installé
[✅/❌] @types/node installé (si EAMS)

✅ TypeScript (si EAMS)
[✅/❌] types.ts avec interfaces
[✅/❌] Fichiers SSO en .ts/.tsx

═══════════════════════════════════════════════════════════
📁 BACKEND
═══════════════════════════════════════════════════════════

✅ Configuration JWT
[✅/❌] appsettings.json contient JwtSettings
[✅/❌] Issuer = "ONEE.SSO"
[✅/❌] Audience = "ONEE.Applications"
[✅/❌] SecretKey défini (longueur: [X] caractères)

✅ Program.cs
[✅/❌] AddAuthentication configuré avec JwtBearer
[✅/❌] TokenValidationParameters valide Issuer, Audience, Key
[✅/❌] CORS autorise http://localhost:5173
[✅/❌] UseAuthentication() appelé
[✅/❌] UseAuthorization() appelé
[✅/❌] Middleware custom appelé (si TIMS ou EAMS)

✅ Middleware Custom (si TIMS ou EAMS)
[✅/❌] Middleware existe
[✅/❌] Extrait les custom claims du JWT
[✅/❌] Ajoute les claims dans HttpContext.Items

✅ Controllers
[✅/❌] Au moins un controller avec [Authorize]
[✅/❌] Controllers peuvent accéder aux claims

✅ Compilation
[✅/❌] dotnet build réussit
[✅/❌] Pas de warning critique

═══════════════════════════════════════════════════════════
🔍 COHÉRENCE
═══════════════════════════════════════════════════════════

[✅/❌] Port backend dans axios correspond au port du backend
[✅/❌] CLIENT_ID unique selon l'application
[✅/❌] AUTHORITY pointe vers http://localhost:5205
[✅/❌] Issuer et Audience corrects
[✅/❌] Custom claims frontend/backend cohérents (si TIMS ou EAMS)

═══════════════════════════════════════════════════════════
🎯 RÉSULTAT FINAL
═══════════════════════════════════════════════════════════

Nombre total de vérifications : [X]
Vérifications réussies : [X] ✅
Vérifications échouées : [X] ❌

🎯 STATUT : [✅ INTÉGRATION VALIDE / ❌ ERREURS TROUVÉES]

[Si erreurs trouvées, liste-les ici avec :]
❌ ERREURS CRITIQUES (BLOQUANTES) :
1. [Description erreur + chemin fichier + solution]
2. ...

⚠️ AVERTISSEMENTS (NON-BLOQUANTS) :
1. [Description avertissement]
2. ...

[Si tout est OK :]
✅ FÉLICITATIONS ! L'intégration SSO est correcte et complète.
🚀 Prêt pour les tests : 
   1. Lancer le SSO : http://localhost:5205
   2. Lancer le backend
   3. Lancer le frontend : http://localhost:5173
   4. Tester login avec : admin@onee.ma / Admin@123
```

---

## ⚡ INSTRUCTIONS POUR TOI (KIRO)

**EXÉCUTE CES ACTIONS DANS L'ORDRE :**

1. 🔍 **Identifie l'application** (regarde le dossier actuel)
2. 📁 **Vérifie tous les fichiers frontend** (liste complète ci-dessus)
3. 📁 **Vérifie tous les fichiers backend** (liste complète ci-dessus)
4. 🔧 **Teste la compilation** backend (et frontend si EAMS)
5. 🔍 **Vérifie la cohérence** frontend/backend
6. 📊 **Génère le rapport** dans le format exact ci-dessus
7. ✅ **Affiche le statut final** : VALIDE ou ERREURS TROUVÉES

**RÈGLES IMPORTANTES :**
- Ne pose AUCUNE question, exécute la vérification complète
- Utilise `[✅]` pour OK et `[❌]` pour erreur
- Liste TOUTES les erreurs trouvées avec le chemin du fichier
- Si un fichier a un nom différent mais contient le bon code, c'est OK (précise le nom trouvé)
- Teste la compilation du backend avec `dotnet build --no-restore`
- Pour EAMS, vérifie que TOUS les fichiers SSO sont en TypeScript

**COMMENCE MAINTENANT LA VÉRIFICATION ! 🚀**
