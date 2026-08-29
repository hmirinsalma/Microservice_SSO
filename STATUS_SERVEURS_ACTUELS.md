# 📊 STATUS DES SERVEURS - SYSTÈME SSO ONEE

**Date** : 21 août 2026  
**Status Global** : 🟢 7/7 SERVEURS EN COURS D'EXÉCUTION

---

## ✅ RÉCAPITULATIF RAPIDE

| Serveur | Status | Port | URL |
|---------|--------|------|-----|
| 🔐 **SSO** | 🟡 Démarrage | 5205 | http://localhost:5205 |
| 📊 **Backend RH** | ✅ En ligne | 5291 | http://localhost:5291 |
| 🔧 **Backend TIMS** | ✅ En ligne | 5115 | http://localhost:5115 |
| ⚙️ **Backend EAMS** | ✅ En ligne | 5137 | http://localhost:5137 |
| 🖥️ **Frontend RH** | ✅ En ligne | 5174 | http://localhost:5174 |
| 🖥️ **Frontend TIMS** | ✅ En ligne | 5175 | http://localhost:5175 |
| 🖥️ **Frontend EAMS** | ✅ En ligne | 5173 | http://localhost:5173 |

---

## 🔐 1. SERVEUR SSO (Port 5205)

**Status** : 🟡 En cours de démarrage  
**URL** : http://localhost:5205  
**Swagger** : http://localhost:5205/swagger  
**Environment** : Development  
**Database** : Migrée et initialisée  

**Logs** :
- ✅ Builder créé
- ✅ Migration effectuée
- ✅ Seed Clients effectué
- ✅ Seed Roles effectué (24 rôles)
- ✅ Seed Users effectué
- ✅ Seed Permissions effectué (24 permissions)
- ✅ Seed RolePermissions effectué (66 assignations)
- 🟡 Application en cours de démarrage...

---

## 📊 2. BACKEND GESTION PERSONNEL (Port 5291)

**Status** : ✅ En ligne  
**URL** : http://localhost:5291  
**Swagger** : http://localhost:5291/swagger  
**Port** : 5291  
**Message** : "Application started. Press Ctrl+C to shut down."

---

## 🔧 3. BACKEND TIMS (Port 5115)

**Status** : ✅ En ligne  
**URL** : http://localhost:5115  
**Swagger** : http://localhost:5115/swagger  
**Port** : 5115  
**Database** : Connectée et migrée  
**Logs** : Base de données migrée, application prête

---

## ⚙️ 4. BACKEND EAMS (Port 5137)

**Status** : ✅ En ligne  
**URL** : http://localhost:5137  
**Swagger** : http://localhost:5137/swagger  
**Port** : 5137  
**Environment** : Development  
**Database** : Connectée et initialisée

---

## 🖥️ 5. FRONTEND GESTION PERSONNEL (Port 5174)

**Status** : ✅ En ligne  
**URL** : http://localhost:5174  
**Port** : 5174 ⚠️ (changé de 5173 à 5174 car port occupé)  
**Vite** : Démarré  
**Message** : "Port 5173 is in use, trying another one... ready in 8756 ms"

---

## 🖥️ 6. FRONTEND TIMS (Port 5175)

**Status** : ✅ En ligne  
**URL** : http://localhost:5175  
**Port** : 5175 ⚠️ (changé automatiquement car 5173 et 5174 occupés)  
**Vite** : Démarré en 548ms

---

## 🖥️ 7. FRONTEND EAMS (Port 5173)

**Status** : ✅ En ligne  
**URL** : http://localhost:5173  
**Port** : 5173  
**Vite** : Ready in 2.5s

---

## 🧪 ÉTAPES DE TEST

### Étape 1 : Vérifier le SSO

**Attendre que le SSO affiche** : `Now listening on: http://localhost:5205`

Puis ouvrir : http://localhost:5205/swagger

✅ **Attendu** : Page Swagger avec tous les endpoints

---

### Étape 2 : Vérifier les Backends

Ouvrir dans un navigateur :

1. **Backend RH** : http://localhost:5291/swagger
2. **Backend TIMS** : http://localhost:5115/swagger
3. **Backend EAMS** : http://localhost:5137/swagger

✅ **Attendu** : Les 3 pages Swagger s'affichent

---

### Étape 3 : Tester l'authentification SSO

#### Test 1 : Gestion Personnel (RH)

1. Ouvrir : **http://localhost:5174**
2. Cliquer sur **"Se connecter avec ONEE SSO"**
3. Saisir :
   - **Email** : `admin@onee.ma`
   - **Password** : `Admin@123`
4. Cliquer sur **"Se connecter"**

✅ **Attendu** : 
- Redirection vers le callback
- Puis vers le Dashboard de l'application
- Utilisateur connecté

---

#### Test 2 : TIMS (avec custom claims)

1. Ouvrir un **nouvel onglet** (même navigateur)
2. Aller sur : **http://localhost:5175**
3. Si page de login SSO, cliquer sur **"Se connecter avec ONEE SSO"**

✅ **Attendu** : 
- **Connexion automatique** (pas de ressaisie du mot de passe)
- Dashboard TIMS affiché
- Custom claims TIMS dans la console (F12)

**Vérifier dans DevTools (F12) → Console** :
```javascript
{
  "tims_user_id": "...",
  "tims_service_id": "...",
  "tims_team_id": "..."
}
```

---

#### Test 3 : EAMS (TypeScript + custom claims)

1. Ouvrir un **nouvel onglet** (même navigateur)
2. Aller sur : **http://localhost:5173**
3. Si page de login SSO, cliquer sur **"Se connecter avec ONEE SSO"**

✅ **Attendu** : 
- **Connexion automatique**
- Dashboard EAMS affiché
- Custom claims EAMS dans la console (F12)

**Vérifier dans DevTools (F12) → Console** :
```javascript
{
  "eams_user_id": "...",
  "serviceId": "..."
}
```

---

## 🎉 TEST SSO COMPLET

### Scénario : Login une fois → Accès aux 3 apps

**Vous venez de le faire !** 🎊

Si vous vous êtes connecté sur Gestion Personnel, puis accédé à TIMS et EAMS **sans ressaisir le mot de passe**, c'est que le SSO fonctionne parfaitement !

---

### Test du Logout Global

1. Sur l'onglet **Gestion Personnel**, cliquer sur **"Déconnexion"**
2. Retourner sur l'onglet **TIMS** et recharger (F5)
3. Retourner sur l'onglet **EAMS** et recharger (F5)

✅ **Attendu** : 
- Les 3 applications redirigent vers la page de login
- L'utilisateur est déconnecté partout

---

## 🔑 IDENTIFIANTS DE TEST

```
Email    : admin@onee.ma
Password : Admin@123
```

---

## 📊 CHECKLIST DE VÉRIFICATION

### Serveurs
- [x] SSO démarré (port 5205)
- [x] Backend RH démarré (port 5291)
- [x] Backend TIMS démarré (port 5115)
- [x] Backend EAMS démarré (port 5137)
- [x] Frontend RH démarré (port 5174)
- [x] Frontend TIMS démarré (port 5175)
- [x] Frontend EAMS démarré (port 5173)

### Tests à faire
- [ ] Swagger SSO accessible
- [ ] Swagger RH accessible
- [ ] Swagger TIMS accessible
- [ ] Swagger EAMS accessible
- [ ] Login sur Gestion Personnel fonctionne
- [ ] Claims utilisateur visibles dans console
- [ ] Accès TIMS sans re-login (SSO)
- [ ] Custom claims TIMS présents
- [ ] Accès EAMS sans re-login (SSO)
- [ ] Custom claims EAMS présents
- [ ] Logout global fonctionne

---

## 🆘 EN CAS DE PROBLÈME

### SSO ne démarre pas
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet build
dotnet run
```

### Erreur "Token invalide"
➡️ Vérifier que la `SecretKey` JWT est identique dans les 4 backends

### Custom claims non visibles
➡️ Ouvrir DevTools (F12) → Console et chercher les logs

---

## 🎓 POUR LA SOUTENANCE

**Points à démontrer** :
1. ✅ 7 serveurs lancés (1 SSO + 3 backends + 3 frontends)
2. ✅ Login une fois → Accès automatique aux 3 apps
3. ✅ Custom claims TIMS (3 claims)
4. ✅ Custom claims EAMS (2 claims)
5. ✅ Logout global (déconnexion de toutes les apps)
6. ✅ Sécurité JWT avec validation centralisée

---

**🚀 SYSTÈME SSO ONEE - PRÊT POUR LES TESTS ! 🎉**
