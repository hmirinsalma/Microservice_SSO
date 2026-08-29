# 🧪 GUIDE DE TESTS E2E - SYSTÈME SSO ONEE

## 📋 PRÉ-REQUIS

✅ Tous les serveurs doivent être lancés (7 fenêtres PowerShell)  
✅ Utiliser le script `LANCER_TOUS_LES_SERVEURS.ps1` pour tout démarrer automatiquement

---

## 🚀 PHASE 1 : VÉRIFICATION DES SERVEURS

### Étape 1.1 : Vérifier que tous les serveurs sont démarrés

**Vérifier dans les 7 fenêtres PowerShell ouvertes :**

| Serveur | Port | Message de succès attendu |
|---------|------|---------------------------|
| 🔐 **SSO** | 5205 | `Now listening on: http://localhost:5205` |
| 📊 **Backend RH** | 5291 | `Now listening on: http://localhost:5291` |
| 🔧 **Backend TIMS** | 5115 | `Now listening on: http://localhost:5115` |
| ⚙️ **Backend EAMS** | 5137 | `Now listening on: http://localhost:5137` |
| 🖥️ **Frontend RH** | 5173 | `Local: http://localhost:5173` |
| 🖥️ **Frontend TIMS** | Auto | `Local: http://localhost:XXXX` (noter le port) |
| 🖥️ **Frontend EAMS** | Auto | `Local: http://localhost:XXXX` (noter le port) |

**⚠️ IMPORTANT** : Noter les ports des frontends TIMS et EAMS (Vite choisira automatiquement un port libre)

---

### Étape 1.2 : Tester les endpoints backend

Ouvrir un navigateur et tester :

**SSO (Swagger UI)**
```
http://localhost:5205/swagger
```
✅ Attendu : Page Swagger avec tous les endpoints SSO

**Backend RH**
```
http://localhost:5291/swagger
```
✅ Attendu : Page Swagger de l'API Gestion Personnel

**Backend TIMS**
```
http://localhost:5115/swagger
```
✅ Attendu : Page Swagger de l'API TIMS

**Backend EAMS**
```
http://localhost:5137/swagger
```
✅ Attendu : Page Swagger de l'API EAMS

---

## 🧪 PHASE 2 : TEST APPLICATION GESTION PERSONNEL

### Test 2.1 : Accès à l'application

1. Ouvrir un navigateur (mode navigation privée recommandé)
2. Aller sur : `http://localhost:5173`

✅ **Attendu** : Page d'accueil de Gestion Personnel avec bouton "Se connecter avec ONEE SSO"

---

### Test 2.2 : Login SSO

1. Cliquer sur le bouton **"Se connecter avec ONEE SSO"**
2. Vous êtes redirigé vers : `http://localhost:5205/connect/authorize?...`

✅ **Attendu** : Page de login du serveur SSO

3. Saisir les identifiants :
   - **Email** : `admin@onee.ma`
   - **Password** : `Admin@123`

4. Cliquer sur **"Se connecter"**

✅ **Attendu** : 
- Redirection vers `http://localhost:5173/callback`
- Puis redirection vers le Dashboard de l'application

---

### Test 2.3 : Vérifier les claims dans la console

1. Ouvrir **DevTools** (F12)
2. Aller dans l'onglet **Console**
3. Chercher les logs :

```
✅ User loaded: {sub: "...", email: "admin@onee.ma", ...}
```

✅ **Attendu** : 
- `sub` : ID utilisateur
- `email` : `admin@onee.ma`
- `name` : `Admin User`
- `roles` : `["AdministrateurRH"]`
- `permissions` : `["USER_UPDATE", "USER_READ", "USER_CREATE", "USER_DELETE"]`

---

### Test 2.4 : Tester un endpoint protégé

1. Dans le Dashboard, naviguer vers une page qui fait un appel API (ex: liste employés)
2. Ouvrir **DevTools** → Onglet **Network**
3. Filtrer par **XHR**
4. Regarder une requête API

✅ **Attendu** dans les **Headers** de la requête :
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

✅ **Attendu** : La requête retourne **200 OK** avec les données

---

### Test 2.5 : Logout

1. Cliquer sur **"Déconnexion"** dans l'application
2. Vous êtes redirigé vers le SSO

✅ **Attendu** : 
- Redirection vers `http://localhost:5205/connect/logout`
- Puis redirection vers la page de login
- localStorage vide (vérifier dans DevTools → Application → Local Storage)

---

## 🧪 PHASE 3 : TEST APPLICATION TIMS (AVEC CUSTOM CLAIMS)

### Test 3.1 : Accès à l'application

1. Ouvrir un **nouvel onglet** (même navigateur)
2. Aller sur : `http://localhost:XXXX` (le port noté pour TIMS)
3. Si route `/login-sso` existe, aller sur : `http://localhost:XXXX/login-sso`

✅ **Attendu** : Page de login SSO TIMS

---

### Test 3.2 : Login SSO avec custom claims TIMS

1. Cliquer sur **"Se connecter avec ONEE SSO"**
2. Si vous êtes **déjà connecté** dans Gestion Personnel :
   - ✅ **Login automatique** sans re-saisir le mot de passe
   - Redirection directe vers le Dashboard TIMS
3. Sinon, saisir : `admin@onee.ma` / `Admin@123`

✅ **Attendu** : Dashboard TIMS s'affiche

---

### Test 3.3 : Vérifier les CUSTOM CLAIMS TIMS

1. Ouvrir **DevTools** → **Console**
2. Chercher les logs :

```javascript
✅ TIMS User loaded: {...}
📋 Custom claims: {
  tims_user_id: "...",
  tims_service_id: "...",
  tims_team_id: "..."
}
```

✅ **Attendu** : Les 3 custom claims TIMS sont présents

---

### Test 3.4 : Vérifier les headers HTTP custom TIMS

1. Ouvrir **DevTools** → **Network**
2. Filtrer par **XHR**
3. Faire une action qui appelle l'API (ex: voir les interventions)
4. Cliquer sur la requête
5. Regarder l'onglet **Headers** → **Request Headers**

✅ **Attendu** :
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
X-TIMS-User-Id: [valeur]
X-TIMS-Service-Id: [valeur]
X-TIMS-Team-Id: [valeur]
```

---

### Test 3.5 : Tester un endpoint SSO Test

1. Aller sur Swagger TIMS : `http://localhost:5115/swagger`
2. Tester **GET /api/testsso/verify-claims**
3. Cliquer sur **"Try it out"** → **"Execute"**
4. Coller le Bearer token (récupéré dans DevTools)

✅ **Attendu** : Réponse 200 avec tous les claims + custom claims TIMS

---

## 🧪 PHASE 4 : TEST APPLICATION EAMS (TYPESCRIPT + CUSTOM CLAIMS)

### Test 4.1 : Accès à l'application

1. Ouvrir un **nouvel onglet** (même navigateur)
2. Aller sur : `http://localhost:XXXX` (le port noté pour EAMS)
3. Si route `/login-sso` existe, aller sur : `http://localhost:XXXX/login-sso`

✅ **Attendu** : Page de login SSO EAMS

---

### Test 4.2 : Login SSO avec custom claims EAMS

1. Cliquer sur **"Se connecter avec ONEE SSO"**
2. Si vous êtes **déjà connecté** :
   - ✅ **Login automatique**
   - Redirection directe vers le Dashboard EAMS

✅ **Attendu** : Dashboard EAMS s'affiche

---

### Test 4.3 : Vérifier les CUSTOM CLAIMS EAMS

1. Ouvrir **DevTools** → **Console**
2. Chercher les logs :

```javascript
✅ EAMS User loaded: {...}
📋 Custom claims: {
  eams_user_id: "...",
  serviceId: "..."
}
```

✅ **Attendu** : Les 2 custom claims EAMS sont présents

---

### Test 4.4 : Vérifier les headers HTTP custom EAMS

1. Ouvrir **DevTools** → **Network**
2. Filtrer par **XHR**
3. Faire une action qui appelle l'API (ex: voir les équipements)
4. Cliquer sur la requête
5. Regarder l'onglet **Headers** → **Request Headers**

✅ **Attendu** :
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
X-EAMS-User-Id: [valeur]
X-EAMS-Service-Id: [valeur]
```

---

### Test 4.5 : Tester les endpoints SSO Test EAMS

Aller sur Swagger EAMS : `http://localhost:5137/swagger`

**Test 1 : GET /api/SsoTest/profile**
1. Cliquer sur **"Try it out"** → **"Execute"**
2. Coller le Bearer token

✅ **Attendu** : Réponse 200 avec tous les claims + custom claims EAMS

**Test 2 : GET /api/SsoTest/equipments**
1. Cliquer sur **"Try it out"** → **"Execute"**
2. Coller le Bearer token

✅ **Attendu** : Réponse 200 avec simulation de filtrage RBAC basé sur `serviceId`

**Test 3 : GET /api/SsoTest/admin-only**
1. Cliquer sur **"Try it out"** → **"Execute"**
2. Coller le Bearer token

✅ **Attendu** : 
- Si l'utilisateur a le rôle `Admin_Patrimoine` : Réponse 200
- Sinon : Réponse 403 Forbidden

---

## 🧪 PHASE 5 : TEST CROSS-APPLICATION (SSO COMPLET)

### Test 5.1 : Scénario complet - Login une fois, accès aux 3 apps

**Étape 1 : Se connecter sur Gestion Personnel**
1. Ouvrir un **nouveau navigateur en mode privé**
2. Aller sur `http://localhost:5173` (Gestion Personnel)
3. Se connecter : `admin@onee.ma` / `Admin@123`

✅ **Attendu** : Dashboard Gestion Personnel affiché

**Étape 2 : Ouvrir TIMS (nouvel onglet)**
1. Ouvrir un **nouvel onglet** (même navigateur)
2. Aller sur le port de TIMS
3. Si demandé, cliquer sur "Se connecter avec SSO"

✅ **Attendu** : **Login automatique** sans ressaisir le mot de passe → Dashboard TIMS

**Étape 3 : Ouvrir EAMS (nouvel onglet)**
1. Ouvrir un **nouvel onglet** (même navigateur)
2. Aller sur le port de EAMS
3. Si demandé, cliquer sur "Se connecter avec SSO"

✅ **Attendu** : **Login automatique** → Dashboard EAMS

**Étape 4 : Se déconnecter d'une app**
1. Sur l'onglet **Gestion Personnel**, cliquer sur **"Déconnexion"**

✅ **Attendu** : Redirection vers le SSO

**Étape 5 : Vérifier les autres apps**
1. Retourner sur l'onglet **TIMS**
2. Recharger la page (F5)

✅ **Attendu** : L'utilisateur est déconnecté → Redirection vers login

3. Retourner sur l'onglet **EAMS**
4. Recharger la page (F5)

✅ **Attendu** : L'utilisateur est déconnecté → Redirection vers login

**🎉 RÉSULTAT** : Le SSO fonctionne parfaitement ! Login une fois → Accès aux 3 apps, Logout une fois → Déconnexion de toutes les apps

---

## 🧪 PHASE 6 : TESTS AVANCÉS (OPTIONNELS)

### Test 6.1 : Refresh Token (renouvellement automatique)

1. Se connecter sur une application
2. Attendre **1 heure** (expiration du token)
3. Faire une action qui appelle l'API

✅ **Attendu** : Le refresh token est utilisé automatiquement, un nouveau access token est généré, l'appel API réussit

---

### Test 6.2 : Account Lockout (5 tentatives échouées)

1. Aller sur le SSO : `http://localhost:5205/swagger`
2. Tester **POST /api/Auth/login** avec un mauvais mot de passe
3. Répéter **5 fois**

✅ **Attendu** : Après 5 tentatives, le compte est verrouillé (erreur 403)

---

### Test 6.3 : Permissions par rôle

1. Se connecter avec un utilisateur ayant le rôle `EmployeRH` (si configuré)
2. Essayer d'accéder à une route admin

✅ **Attendu** : Erreur 403 Forbidden ou message "Accès refusé"

---

## 📊 CHECKLIST FINALE

### ✅ Tests Réussis

- [ ] Serveur SSO démarre (port 5205)
- [ ] Backend RH démarre (port 5291)
- [ ] Backend TIMS démarre (port 5115)
- [ ] Backend EAMS démarre (port 5137)
- [ ] Frontend RH démarre (port 5173)
- [ ] Frontend TIMS démarre
- [ ] Frontend EAMS démarre
- [ ] Login SSO sur Gestion Personnel fonctionne
- [ ] Claims utilisateur présents dans la console (RH)
- [ ] Appels API protégés fonctionnent (RH)
- [ ] Logout fonctionne (RH)
- [ ] Login SSO sur TIMS fonctionne
- [ ] Custom claims TIMS présents (tims_user_id, tims_service_id, tims_team_id)
- [ ] Headers HTTP custom TIMS envoyés (X-TIMS-*)
- [ ] Endpoint /api/testsso/verify-claims retourne les claims
- [ ] Login SSO sur EAMS fonctionne
- [ ] Custom claims EAMS présents (eams_user_id, serviceId)
- [ ] Headers HTTP custom EAMS envoyés (X-EAMS-*)
- [ ] Endpoint /api/SsoTest/profile retourne les claims
- [ ] Login une fois → Accès aux 3 apps (SSO complet)
- [ ] Logout une fois → Déconnexion des 3 apps

---

## 🎯 RÉSULTAT ATTENDU

Si tous les tests passent :

```
═══════════════════════════════════════════════════════════
🎉 FÉLICITATIONS ! LE SYSTÈME SSO ONEE EST FONCTIONNEL !
═══════════════════════════════════════════════════════════

✅ Login une fois → Accès aux 3 applications
✅ Custom claims TIMS et EAMS fonctionnent
✅ Logout révoque toutes les sessions
✅ Les tokens sont automatiquement ajoutés aux requêtes
✅ Les backends valident correctement les JWT

🚀 SYSTÈME PRÊT POUR LA PRODUCTION ET LA SOUTENANCE ! 🎉
```

---

## 🆘 EN CAS DE PROBLÈME

### Erreur : Port déjà utilisé
**Solution** : Arrêter le processus sur ce port ou changer le port dans `launchSettings.json`

### Erreur : Token invalide
**Solution** : Vérifier que la `SecretKey` JWT est identique dans les 3 backends

### Erreur : CORS
**Solution** : Vérifier que le backend autorise l'origine du frontend

### Erreur : Custom claims non présents
**Solution** : 
1. Vérifier que les scopes sont dans `authConfig.js/ts`
2. Vérifier que le middleware custom est appelé après `UseAuthentication()`

### Erreur : Compilation backend échouée
**Solution** : `dotnet build` dans le dossier du backend et regarder les erreurs

---

## 📞 SUPPORT

Si problème persistant :
1. Vérifier les logs dans les 7 fenêtres PowerShell
2. Vérifier la console du navigateur (F12)
3. Vérifier les headers HTTP dans Network (F12)
4. Relire le `RAPPORT_VERIFICATION_FINAL.md`

**BON TEST ! 🚀**
