# 🔍 PROMPT DE VÉRIFICATION - INTÉGRATION SSO COMPLÈTE

## CONTEXTE

J'ai intégré 3 applications avec mon serveur SSO ONEE (http://localhost:5205). Je veux vérifier que **TOUT est correctement configuré** avant de faire les tests finaux.

---

## 🎯 TON TRAVAIL : VÉRIFIER LES 3 INTÉGRATIONS SSO

Tu dois vérifier l'intégration SSO de ces 3 applications :

### Application 1 : GESTION PERSONNEL (RH)
- **Dossier Frontend** : `c:\Users\XPS\Desktop\Gestion du Prsonnel\frontend`
- **Dossier Backend** : `c:\Users\XPS\Desktop\Gestion du Prsonnel\backend\API-GestionPersonnel`
- **Port Frontend** : 5173
- **Port Backend** : 5291
- **ClientId** : `gestion-personnel`
- **Scopes** : `openid profile email roles offline_access gestion-personnel`

### Application 2 : TIMS (Gestion des Interventions)
- **Dossier Frontend** : `c:\Users\XPS\Desktop\gestion des interventions\frontend`
- **Dossier Backend** : `c:\Users\XPS\Desktop\gestion des interventions\backend\TIMS.API`
- **Port Frontend** : 5173
- **Port Backend** : 5115
- **ClientId** : `tims-app`
- **Scopes** : `openid profile email roles offline_access tims tims_user_id tims_service_id tims_team_id`
- **Custom Claims** : `tims_user_id`, `tims_service_id`, `tims_team_id`

### Application 3 : EAMS (Gestion des Équipements)
- **Dossier Frontend** : `c:\Users\XPS\Desktop\gestion des equipements\frontend`
- **Dossier Backend** : `c:\Users\XPS\Desktop\gestion des equipements\backend\EAMS.API`
- **Port Frontend** : 5173
- **Port Backend** : 5137
- **ClientId** : `eams-spa`
- **Scopes** : `openid profile email roles offline_access eams eams_user_id serviceId`
- **Custom Claims** : `eams_user_id`, `serviceId`
- **TypeScript** : Oui

---

## ✅ VÉRIFICATIONS À EFFECTUER

### POUR CHAQUE APPLICATION :

#### 1️⃣ VÉRIFICATION FRONTEND

**Fichiers obligatoires à vérifier :**
- [ ] `src/auth/authConfig.js` (ou `.ts` pour EAMS) existe
- [ ] `src/auth/authService.js` (ou `.ts` pour EAMS) existe
- [ ] `src/pages/Login.jsx` (ou `.tsx`) ou `LoginSSO.jsx` existe
- [ ] `src/pages/Callback.jsx` (ou `.tsx`) existe
- [ ] `src/components/ProtectedRoute.jsx` (ou `.tsx`) ou `ProtectedRouteSSO.jsx` existe
- [ ] `src/api/axiosConfig.js` (ou `.ts`) ou `axiosInstanceSSO.ts` existe
- [ ] `public/silent-renew.html` existe

**Vérifications dans authConfig :**
- [ ] `CLIENT_ID` correspond au bon ClientId
- [ ] `CLIENT_SECRET` est défini
- [ ] `REDIRECT_URI` pointe vers `http://localhost:5173/callback`
- [ ] `AUTHORITY` = `http://localhost:5205`
- [ ] `scope` contient tous les scopes requis (vérifier custom scopes pour TIMS et EAMS)

**Vérifications dans authService :**
- [ ] Méthode `login()` existe
- [ ] Méthode `completeLogin()` existe
- [ ] Méthode `logout()` existe
- [ ] Méthode `getAccessToken()` existe
- [ ] Pour TIMS : Méthodes `getTimsUserId()`, `getTimsServiceId()`, `getTimsTeamId()` existent
- [ ] Pour EAMS : Méthodes `getEamsUserId()`, `getServiceId()` existent

**Vérifications dans axiosConfig :**
- [ ] Interceptor ajoute le token dans les headers `Authorization: Bearer`
- [ ] Pour TIMS : Interceptor ajoute headers `X-TIMS-User-Id`, `X-TIMS-Service-Id`, `X-TIMS-Team-Id`
- [ ] Pour EAMS : Interceptor ajoute headers `X-EAMS-User-Id`, `X-EAMS-Service-Id`

**Vérifications dans package.json :**
- [ ] Dépendance `oidc-client-ts` installée
- [ ] Dépendance `react-router-dom` installée
- [ ] Pour EAMS : Dépendance `@types/node` installée

**Vérifications TypeScript (EAMS uniquement) :**
- [ ] `src/auth/types.ts` existe avec interfaces `UserProfile`, `EamsContext`
- [ ] Tous les fichiers SSO utilisent `.ts` ou `.tsx` (pas `.js`)

---

#### 2️⃣ VÉRIFICATION BACKEND

**Fichiers obligatoires à vérifier :**
- [ ] `appsettings.json` contient section `JwtSettings`
- [ ] `Program.cs` configure JWT Authentication
- [ ] Pour TIMS : `Middlewares/TimsContextMiddleware.cs` existe
- [ ] Pour EAMS : `Middlewares/EamsContextMiddleware.cs` existe

**Vérifications dans appsettings.json :**
- [ ] `JwtSettings.Issuer` = `ONEE.SSO`
- [ ] `JwtSettings.Audience` = `ONEE.Applications`
- [ ] `JwtSettings.SecretKey` est défini (32+ caractères)
- [ ] La `SecretKey` est **identique** dans les 3 backends

**Vérifications dans Program.cs :**
- [ ] `builder.Services.AddAuthentication()` configuré avec `JwtBearer`
- [ ] `TokenValidationParameters` valide `Issuer`, `Audience`, `IssuerSigningKey`
- [ ] CORS autorise `http://localhost:5173`
- [ ] `app.UseAuthentication()` est appelé
- [ ] `app.UseAuthorization()` est appelé
- [ ] Pour TIMS : `app.UseTimsContext()` est appelé après `UseAuthentication()`
- [ ] Pour EAMS : `app.UseEamsContext()` est appelé après `UseAuthentication()`

**Vérifications dans les Middlewares custom (TIMS et EAMS) :**
- [ ] TIMS : Extrait `tims_user_id`, `tims_service_id`, `tims_team_id` du JWT
- [ ] EAMS : Extrait `eams_user_id`, `serviceId` du JWT
- [ ] Les claims sont ajoutés dans `HttpContext.Items`

**Vérifications des Controllers :**
- [ ] Au moins un controller a `[Authorize]`
- [ ] Les controllers peuvent accéder aux claims via `User.FindFirst("sub")`
- [ ] Pour TIMS : Controllers peuvent accéder à `HttpContext.Items["TimsUserId"]`
- [ ] Pour EAMS : Controllers peuvent accéder à `HttpContext.Items["EamsUserId"]`

---

#### 3️⃣ VÉRIFICATION DE LA COMPILATION

**Backend :**
- [ ] `dotnet build` réussit sans erreur dans chaque backend
- [ ] Aucun warning critique sur JWT ou Authentication

**Frontend :**
- [ ] `npm install` (ou `npm ci`) réussit
- [ ] Pour EAMS : `npm run build` compile TypeScript sans erreur critique sur les fichiers SSO

---

#### 4️⃣ VÉRIFICATION DE LA COHÉRENCE

**Entre Frontend et Backend :**
- [ ] Les ports configurés dans `axiosConfig` correspondent aux ports des backends
- [ ] Le `CLIENT_ID` dans le frontend correspond à celui configuré dans le SSO
- [ ] La `SecretKey` JWT est **identique** dans les 3 backends

**Entre les 3 applications et le serveur SSO :**
- [ ] Les 3 `CLIENT_ID` sont différents : `gestion-personnel`, `tims-app`, `eams-spa`
- [ ] Tous pointent vers le même `AUTHORITY` : `http://localhost:5205`
- [ ] Tous utilisent la même `Issuer` : `ONEE.SSO`
- [ ] Tous utilisent la même `Audience` : `ONEE.Applications`

---

## 📋 FORMAT DE RÉPONSE ATTENDU

Pour chaque application, génère un rapport dans ce format :

```
═══════════════════════════════════════════════════════════
✅ APPLICATION : GESTION PERSONNEL (RH)
═══════════════════════════════════════════════════════════

📁 FRONTEND (c:\Users\XPS\Desktop\Gestion du Prsonnel\frontend)
✅ authConfig.js trouvé - CLIENT_ID: gestion-personnel ✅
✅ authService.js trouvé - Toutes les méthodes présentes ✅
✅ Login.jsx trouvé ✅
✅ Callback.jsx trouvé ✅
✅ ProtectedRoute.jsx trouvé ✅
✅ axiosConfig.js trouvé - Interceptor OK ✅
✅ silent-renew.html trouvé ✅
✅ package.json - oidc-client-ts installé ✅
✅ package.json - react-router-dom installé ✅

📁 BACKEND (c:\Users\XPS\Desktop\Gestion du Prsonnel\backend\API-GestionPersonnel)
✅ appsettings.json - JwtSettings configuré ✅
✅ Program.cs - JWT Authentication configuré ✅
✅ Program.cs - CORS configuré ✅
✅ Program.cs - UseAuthentication() présent ✅
✅ Compilation : dotnet build réussit ✅

🔍 COHÉRENCE
✅ Port backend dans axiosConfig : 5291 ✅
✅ CLIENT_ID unique : gestion-personnel ✅
✅ AUTHORITY : http://localhost:5205 ✅
✅ Issuer : ONEE.SSO ✅
✅ Audience : ONEE.Applications ✅

🎯 STATUT : ✅ INTÉGRATION VALIDE - PRÊTE POUR TESTS

---

═══════════════════════════════════════════════════════════
✅ APPLICATION : TIMS (Gestion des Interventions)
═══════════════════════════════════════════════════════════

📁 FRONTEND (c:\Users\XPS\Desktop\gestion des interventions\frontend)
✅ authConfig.js trouvé - CLIENT_ID: tims-app ✅
✅ authConfig.js - Custom scopes TIMS présents ✅
✅ authService.js - Méthodes custom TIMS présentes ✅
✅ LoginSSO.jsx trouvé ✅
✅ Callback.jsx trouvé ✅
✅ ProtectedRouteSSO.jsx trouvé ✅
✅ axiosInstanceSSO.js - Headers custom X-TIMS-* configurés ✅
✅ silent-renew.html trouvé ✅
✅ package.json - Dépendances OK ✅

📁 BACKEND (c:\Users\XPS\Desktop\gestion des interventions\backend\TIMS.API)
✅ appsettings.json - JwtSettings configuré ✅
✅ Program.cs - JWT Authentication configuré ✅
✅ TimsContextMiddleware.cs trouvé ✅
✅ TimsContextMiddleware - Extraction custom claims OK ✅
✅ Program.cs - UseTimsContext() appelé ✅
✅ Compilation : dotnet build réussit ✅

🔍 CUSTOM CLAIMS TIMS
✅ Frontend envoie : X-TIMS-User-Id, X-TIMS-Service-Id, X-TIMS-Team-Id ✅
✅ Backend extrait : tims_user_id, tims_service_id, tims_team_id ✅
✅ Disponibles dans HttpContext.Items ✅

🎯 STATUT : ✅ INTÉGRATION VALIDE - PRÊTE POUR TESTS

---

═══════════════════════════════════════════════════════════
✅ APPLICATION : EAMS (Gestion des Équipements)
═══════════════════════════════════════════════════════════

📁 FRONTEND (c:\Users\XPS\Desktop\gestion des equipements\frontend)
✅ types.ts trouvé - Interfaces TypeScript OK ✅
✅ authConfig.ts trouvé - CLIENT_ID: eams-spa ✅
✅ authConfig.ts - Custom scopes EAMS présents ✅
✅ authService.ts - Méthodes custom EAMS présentes ✅
✅ LoginSSO.tsx trouvé ✅
✅ Callback.tsx trouvé ✅
✅ ProtectedRouteSSO.tsx trouvé ✅
✅ axiosInstanceSSO.ts - Headers custom X-EAMS-* configurés ✅
✅ silent-renew.html trouvé ✅
✅ package.json - @types/node installé ✅
✅ TypeScript : Tous fichiers SSO en .ts/.tsx ✅

📁 BACKEND (c:\Users\XPS\Desktop\gestion des equipements\backend\EAMS.API)
✅ appsettings.json - JwtSettings configuré ✅
✅ Program.cs - JWT Authentication configuré ✅
✅ EamsContextMiddleware.cs trouvé ✅
✅ EamsContextMiddleware - Extraction custom claims OK ✅
✅ Program.cs - UseEamsContext() appelé ✅
✅ Compilation : dotnet build réussit ✅

🔍 CUSTOM CLAIMS EAMS
✅ Frontend envoie : X-EAMS-User-Id, X-EAMS-Service-Id ✅
✅ Backend extrait : eams_user_id, serviceId ✅
✅ Disponibles dans HttpContext.Items ✅

🎯 STATUT : ✅ INTÉGRATION VALIDE - PRÊTE POUR TESTS

---

═══════════════════════════════════════════════════════════
🎉 RÉSUMÉ GLOBAL
═══════════════════════════════════════════════════════════

✅ GESTION PERSONNEL : INTÉGRATION VALIDE
✅ TIMS : INTÉGRATION VALIDE
✅ EAMS : INTÉGRATION VALIDE

🔒 SÉCURITÉ :
✅ Les 3 backends utilisent la même SecretKey JWT
✅ Les 3 frontends pointent vers le même SSO
✅ Les 3 CLIENT_ID sont uniques

📊 STATISTIQUES :
- Fichiers frontend créés : 21
- Fichiers backend modifiés/créés : 9
- Custom claims configurés : 5 (TIMS: 3, EAMS: 2)
- Applications TypeScript : 1 (EAMS)

🎯 STATUT FINAL : ✅ PRÊT POUR LES TESTS E2E

🚀 PROCHAINE ÉTAPE :
1. Lancer le serveur SSO : http://localhost:5205
2. Lancer les 3 backends
3. Lancer les 3 frontends
4. Tester le login SSO sur chaque application
5. Vérifier les custom claims dans la console
```

---

## ⚠️ SI DES ERREURS SONT TROUVÉES

Pour chaque erreur trouvée, indique :
- ❌ Fichier manquant ou incorrect
- 📍 Chemin du fichier
- 🔧 Action corrective nécessaire
- ⚡ Niveau de criticité (BLOQUANT / MINEUR)

---

## 🎯 INSTRUCTIONS POUR TOI (KIRO)

**EXÉCUTE UNE VÉRIFICATION COMPLÈTE :**

1. ✅ Vérifie tous les fichiers frontend des 3 applications
2. ✅ Vérifie tous les fichiers backend des 3 applications
3. ✅ Vérifie la cohérence entre frontend/backend
4. ✅ Vérifie la cohérence entre les 3 applications
5. ✅ Teste la compilation des backends (`dotnet build --no-restore`)
6. ✅ Vérifie les custom claims TIMS et EAMS
7. ✅ Génère un rapport détaillé dans le format ci-dessus

**NE POSE PAS DE QUESTIONS, VÉRIFIE TOUT ET GÉNÈRE LE RAPPORT !**

---

## 📌 NOTES IMPORTANTES

- Si un fichier est trouvé avec un nom différent (ex: `LoginSSO.jsx` au lieu de `Login.jsx`), c'est OK tant qu'il contient le bon code
- Pour EAMS, tous les fichiers SSO doivent être en TypeScript (.ts/.tsx)
- La `SecretKey` JWT DOIT être identique dans les 3 backends
- Les custom scopes doivent être présents dans `authConfig` de TIMS et EAMS
- Les middlewares custom doivent être appelés APRÈS `UseAuthentication()`

🎯 **Objectif** : M'assurer que TOUT est correctement configuré avant les tests finaux ! 🚀
