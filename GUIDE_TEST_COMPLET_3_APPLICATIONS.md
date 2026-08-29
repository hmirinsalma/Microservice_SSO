# 🎯 GUIDE DE TEST COMPLET - 3 APPLICATIONS SSO

## 📋 RÉSUMÉ RAPIDE

Ce guide vous permet de lancer et tester les **3 applications** intégrées au SSO :
- 🧑 **Gestion Personnel** (RH)
- 🔧 **TIMS** (Gestion des Interventions)
- ⚙️ **EAMS** (Gestion des Équipements)

---

## 🚀 ÉTAPE 1 : LANCER TOUS LES SERVEURS

### Option A : Script Automatique (RECOMMANDÉ) ⭐

```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO
.\LANCER_TOUS_LES_SERVEURS.ps1
```

✅ **Ce script lance automatiquement** :
- 1 serveur SSO (port 5205)
- 3 backends (ports 5291, 5115, 5137)
- 3 frontends (ports auto)

⏳ **Attendre 30-40 secondes** que tous les serveurs démarrent.

---

### Option B : Lancement Manuel (7 terminaux)

Si le script ne fonctionne pas, ouvrir **7 terminaux PowerShell** :

#### Terminal 1 : SSO (Port 5205)
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet run
```
✅ **Attendu** : `Now listening on: http://localhost:5205`

#### Terminal 2 : Backend Gestion Personnel (Port 5291)
```powershell
cd "c:\Users\XPS\Desktop\Gestion du Prsonnel\backend\GestionPersonnel.API"
dotnet run
```
✅ **Attendu** : `Now listening on: http://localhost:5291`

#### Terminal 3 : Backend TIMS (Port 5115)
```powershell
cd "c:\Users\XPS\Desktop\gestion des interventions\backend\TIMS.API"
dotnet run
```
✅ **Attendu** : `Now listening on: http://localhost:5115`

#### Terminal 4 : Backend EAMS (Port 5137)
```powershell
cd "c:\Users\XPS\Desktop\gestion des equipements\backend\ONEE.EAMS.API"
dotnet run
```
✅ **Attendu** : `Now listening on: http://localhost:5137`

#### Terminal 5 : Frontend Gestion Personnel (Port 5173)
```powershell
cd "c:\Users\XPS\Desktop\Gestion du Prsonnel\frontend"
npm run dev
```
✅ **Attendu** : `Local: http://localhost:5173`

#### Terminal 6 : Frontend TIMS
```powershell
cd "c:\Users\XPS\Desktop\gestion des interventions\frontend"
npm run dev
```
✅ **Attendu** : `Local: http://localhost:XXXX` ➡️ **NOTER CE PORT**

#### Terminal 7 : Frontend EAMS
```powershell
cd "c:\Users\XPS\Desktop\gestion des equipements\frontend"
npm run dev
```
✅ **Attendu** : `Local: http://localhost:XXXX` ➡️ **NOTER CE PORT**

---

## ✅ ÉTAPE 2 : VÉRIFICATION RAPIDE DES SERVEURS

Ouvrir un navigateur et tester ces URLs :

| Service | URL | Ce que vous devez voir |
|---------|-----|------------------------|
| 🔐 **SSO Swagger** | http://localhost:5205/swagger | Page Swagger avec endpoints Auth |
| 📊 **RH Swagger** | http://localhost:5291/swagger | Page Swagger API Gestion Personnel |
| 🔧 **TIMS Swagger** | http://localhost:5115/swagger | Page Swagger API TIMS |
| ⚙️ **EAMS Swagger** | http://localhost:5137/swagger | Page Swagger API EAMS |
| 🖥️ **Frontend RH** | http://localhost:5173 | Page d'accueil Gestion Personnel |
| 🖥️ **Frontend TIMS** | http://localhost:XXXX | Page d'accueil TIMS |
| 🖥️ **Frontend EAMS** | http://localhost:XXXX | Page d'accueil EAMS |

✅ Si toutes les pages s'affichent ➡️ **TOUS LES SERVEURS SONT OPÉRATIONNELS !**

---

## 🧪 ÉTAPE 3 : TEST 1 - GESTION PERSONNEL (RH)

### 3.1 - Accès à l'application

1. Ouvrir un navigateur **en mode navigation privée**
2. Aller sur : **http://localhost:5173**

✅ **Attendu** : Page de login avec bouton "Se connecter avec ONEE SSO"

---

### 3.2 - Login SSO

1. Cliquer sur **"Se connecter avec ONEE SSO"**
2. Vous êtes redirigé vers le serveur SSO
3. Saisir les identifiants :

```
Email    : admin@onee.ma
Password : Admin@123
```

4. Cliquer sur **"Se connecter"**

✅ **Attendu** : 
- Redirection vers `http://localhost:5173/callback`
- Puis vers le **Dashboard** de Gestion Personnel

---

### 3.3 - Vérifier les claims (Console DevTools)

1. Appuyer sur **F12** (DevTools)
2. Aller dans l'onglet **Console**
3. Chercher les logs qui affichent les infos utilisateur

✅ **Attendu** : Voir ces informations dans la console :
```javascript
{
  "sub": "e47df645-711b-4ffe-893f-b81a7bd4d856",
  "email": "admin@onee.ma",
  "name": "Admin User",
  "roles": ["AdministrateurRH"],
  "permissions": ["USER_UPDATE", "USER_READ", "USER_CREATE", "USER_DELETE"]
}
```

---

### 3.4 - Tester un appel API protégé

1. Dans le Dashboard, naviguer vers une page qui affiche des données (ex: liste des employés)
2. Dans **DevTools** → Onglet **Network**
3. Filtrer par **XHR** ou **Fetch**
4. Cliquer sur une requête API
5. Regarder les **Request Headers**

✅ **Attendu** : Voir le token dans les headers :
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

✅ **Attendu** : La requête retourne **200 OK** avec les données

---

### 3.5 - Tester le Logout

1. Cliquer sur **"Déconnexion"** dans l'application
2. Vous êtes redirigé vers le SSO

✅ **Attendu** : 
- Redirection vers la page de login
- Token supprimé du localStorage (vérifier dans **DevTools → Application → Local Storage**)

---

## 🧪 ÉTAPE 4 : TEST 2 - TIMS (AVEC CUSTOM CLAIMS)

### 4.1 - Accès à l'application

1. Ouvrir un **nouvel onglet** (même navigateur)
2. Aller sur : **http://localhost:XXXX** (le port noté pour TIMS)
3. Si une route `/login-sso` existe, aller directement sur cette page

✅ **Attendu** : Page de login SSO TIMS

---

### 4.2 - Login SSO automatique

1. Cliquer sur **"Se connecter avec ONEE SSO"**

✅ **Attendu** : 
- **Connexion automatique** (pas besoin de re-saisir le mot de passe)
- Redirection vers le Dashboard TIMS

💡 **Pourquoi ?** Vous êtes déjà connecté au SSO depuis Gestion Personnel !

---

### 4.3 - Vérifier les CUSTOM CLAIMS TIMS

1. Ouvrir **DevTools** (F12) → **Console**
2. Chercher les logs affichant les custom claims TIMS

✅ **Attendu** : Voir ces 3 custom claims TIMS :
```javascript
{
  "tims_user_id": "...",      // ID utilisateur dans TIMS
  "tims_service_id": "...",   // ID du service
  "tims_team_id": "..."       // ID de l'équipe
}
```

---

### 4.4 - Vérifier les headers HTTP custom TIMS

1. Dans **DevTools** → **Network**
2. Filtrer par **XHR**
3. Faire une action qui appelle l'API TIMS (ex: voir les interventions)
4. Cliquer sur la requête
5. Regarder les **Request Headers**

✅ **Attendu** : Voir ces headers personnalisés TIMS :
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
X-TIMS-User-Id: [valeur]
X-TIMS-Service-Id: [valeur]
X-TIMS-Team-Id: [valeur]
```

---

### 4.5 - Tester l'endpoint de vérification SSO

1. Aller sur Swagger TIMS : **http://localhost:5115/swagger**
2. Chercher l'endpoint **GET /api/testsso/verify-claims** ou similaire
3. Cliquer sur **"Try it out"**
4. Dans **Authorization**, coller le Bearer token (récupéré dans DevTools → Network)
5. Cliquer sur **"Execute"**

✅ **Attendu** : Réponse **200 OK** avec tous les claims + custom claims TIMS

---

## 🧪 ÉTAPE 5 : TEST 3 - EAMS (TYPESCRIPT + CUSTOM CLAIMS)

### 5.1 - Accès à l'application

1. Ouvrir un **nouvel onglet** (même navigateur)
2. Aller sur : **http://localhost:XXXX** (le port noté pour EAMS)
3. Si une route `/login-sso` existe, aller directement sur cette page

✅ **Attendu** : Page de login SSO EAMS

---

### 5.2 - Login SSO automatique

1. Cliquer sur **"Se connecter avec ONEE SSO"**

✅ **Attendu** : 
- **Connexion automatique**
- Redirection vers le Dashboard EAMS

💡 **C'est magique !** Vous êtes connecté automatiquement aux 3 applications !

---

### 5.3 - Vérifier les CUSTOM CLAIMS EAMS

1. Ouvrir **DevTools** (F12) → **Console**
2. Chercher les logs affichant les custom claims EAMS

✅ **Attendu** : Voir ces 2 custom claims EAMS :
```javascript
{
  "eams_user_id": "...",   // ID utilisateur dans EAMS
  "serviceId": "..."       // ID du service pour filtrage RBAC
}
```

---

### 5.4 - Vérifier les headers HTTP custom EAMS

1. Dans **DevTools** → **Network**
2. Filtrer par **XHR**
3. Faire une action qui appelle l'API EAMS (ex: voir les équipements)
4. Cliquer sur la requête
5. Regarder les **Request Headers**

✅ **Attendu** : Voir ces headers personnalisés EAMS :
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
X-EAMS-User-Id: [valeur]
X-EAMS-Service-Id: [valeur]
```

---

### 5.5 - Tester les endpoints SSO Test EAMS

Aller sur Swagger EAMS : **http://localhost:5137/swagger**

#### Test 1 : GET /api/SsoTest/profile
1. Cliquer sur **"Try it out"** → **"Execute"**
2. Coller le Bearer token
✅ **Attendu** : Réponse **200 OK** avec tous les claims + custom claims EAMS

#### Test 2 : GET /api/SsoTest/equipments
1. Cliquer sur **"Try it out"** → **"Execute"**
2. Coller le Bearer token
✅ **Attendu** : Réponse **200 OK** avec simulation de filtrage RBAC basé sur `serviceId`

---

## 🎉 ÉTAPE 6 : TEST SSO COMPLET - LOGIN UNE FOIS, ACCÈS AUX 3 APPS

### Scénario : Vérifier que le SSO fonctionne entre les 3 applications

**Vous avez déjà fait ce test !** Vous vous êtes connecté sur **Gestion Personnel**, puis vous avez accédé à **TIMS** et **EAMS** sans re-saisir le mot de passe.

✅ **RÉSULTAT ATTENDU** : C'est exactement le comportement du SSO !

---

### Test du Logout Global

1. Retourner sur l'onglet **Gestion Personnel**
2. Cliquer sur **"Déconnexion"**
3. Retourner sur l'onglet **TIMS**
4. Recharger la page (F5)
5. Retourner sur l'onglet **EAMS**
6. Recharger la page (F5)

✅ **Attendu** : 
- Sur TIMS : Redirection vers la page de login
- Sur EAMS : Redirection vers la page de login

💡 **C'est le SSO complet !** Déconnexion d'une app = déconnexion de toutes les apps !

---

## 📊 CHECKLIST DE VÉRIFICATION FINALE

### ✅ Serveurs lancés
- [ ] SSO démarre (port 5205)
- [ ] Backend RH démarre (port 5291)
- [ ] Backend TIMS démarre (port 5115)
- [ ] Backend EAMS démarre (port 5137)
- [ ] Frontend RH démarre (port 5173)
- [ ] Frontend TIMS démarre
- [ ] Frontend EAMS démarre

### ✅ Tests Gestion Personnel (RH)
- [ ] Login SSO fonctionne
- [ ] Claims utilisateur présents dans la console
- [ ] Appels API protégés fonctionnent
- [ ] Token présent dans les headers
- [ ] Logout fonctionne

### ✅ Tests TIMS
- [ ] Login SSO automatique fonctionne
- [ ] Custom claims TIMS présents (tims_user_id, tims_service_id, tims_team_id)
- [ ] Headers HTTP custom TIMS envoyés (X-TIMS-*)
- [ ] Endpoint /api/testsso/verify-claims retourne les claims

### ✅ Tests EAMS
- [ ] Login SSO automatique fonctionne
- [ ] Custom claims EAMS présents (eams_user_id, serviceId)
- [ ] Headers HTTP custom EAMS envoyés (X-EAMS-*)
- [ ] Endpoint /api/SsoTest/profile retourne les claims

### ✅ Test SSO Global
- [ ] Login une fois → Accès aux 3 applications automatiquement
- [ ] Logout une fois → Déconnexion des 3 applications

---

## 🎯 RÉSULTAT FINAL

Si tous les tests passent :

```
═══════════════════════════════════════════════════════════
🎉 FÉLICITATIONS ! LE SYSTÈME SSO ONEE EST FONCTIONNEL !
═══════════════════════════════════════════════════════════

✅ Login une fois → Accès automatique aux 3 applications
✅ Custom claims TIMS et EAMS fonctionnent parfaitement
✅ Logout global révoque toutes les sessions
✅ Les tokens JWT sont automatiquement ajoutés aux requêtes
✅ Les backends valident correctement les JWT
✅ Les middlewares custom extraient les claims correctement

🚀 SYSTÈME PRÊT POUR LA PRODUCTION ET LA SOUTENANCE ! 🎉
```

---

## 🆘 DÉPANNAGE RAPIDE

### ❌ Erreur : "Port already in use"
**Solution** : Arrêter le processus sur le port ou le changer dans `launchSettings.json`

```powershell
# Trouver le processus
netstat -ano | findstr :5205

# Arrêter le processus (remplacer PID)
taskkill /PID [PID] /F
```

---

### ❌ Erreur : "Token invalide" ou "Unauthorized"
**Solution** : Vérifier que tous les backends utilisent la même `SecretKey` JWT

**Fichiers à vérifier** :
- `c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API\appsettings.json`
- `c:\Users\XPS\Desktop\Gestion du Prsonnel\backend\GestionPersonnel.API\appsettings.json`
- `c:\Users\XPS\Desktop\gestion des interventions\backend\TIMS.API\appsettings.json`
- `c:\Users\XPS\Desktop\gestion des equipements\backend\ONEE.EAMS.API\appsettings.json`

✅ **La clé JWT doit être identique** dans les 4 fichiers !

---

### ❌ Erreur : Custom claims non présents
**Solution** :
1. Vérifier que les scopes sont dans `authConfig.js/ts` (frontend)
2. Vérifier que le middleware custom est appelé **après** `UseAuthentication()` (backend)

---

### ❌ Erreur : CORS (Cross-Origin)
**Solution** : Vérifier que chaque backend autorise l'origine de son frontend

Dans `Program.cs` de chaque backend :
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Port du frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

---

## 📞 SUPPORT ET DOCUMENTATION

### 📄 Autres guides disponibles :
- `COMMANDES_MANUELLES.md` - Commandes détaillées de lancement
- `GUIDE_TESTS_E2E.md` - Tests E2E complets
- `RAPPORT_VERIFICATION_FINAL.md` - Rapport d'intégration 100%

### 🔍 Debugging :
1. Vérifier les logs dans les 7 fenêtres PowerShell
2. Vérifier la console du navigateur (F12)
3. Vérifier les headers HTTP dans Network (F12)
4. Vérifier les logs dans `src\ONEE.SSO.API\Logs\log-YYYYMMDD.txt`

---

## 🎓 POUR LA SOUTENANCE

### Points clés à démontrer :

1. **SSO Fonctionnel** : Login une fois → Accès aux 3 apps
2. **Custom Claims** : TIMS et EAMS utilisent des claims personnalisés
3. **Sécurité JWT** : Tokens validés côté backend
4. **Logout Global** : Déconnexion centralisée
5. **Middleware Custom** : Extraction automatique des claims
6. **Headers HTTP** : Propagation automatique des infos utilisateur

### Démonstration recommandée :

1. Lancer tous les serveurs (script PowerShell)
2. Se connecter sur Gestion Personnel
3. Montrer les claims dans DevTools
4. Accéder à TIMS sans re-login
5. Montrer les custom claims TIMS
6. Accéder à EAMS sans re-login
7. Montrer les custom claims EAMS
8. Se déconnecter et montrer que toutes les apps sont déconnectées

---

**🚀 BON TEST ! VOUS ÊTES PRÊT POUR LA SOUTENANCE ! 🎉**
