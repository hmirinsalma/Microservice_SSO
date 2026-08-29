# 🧪 GUIDE DE TEST - FLUX SSO COMPLET

**Date** : 22 août 2026  
**Objectif** : Tester le flux d'authentification unique (SSO) entre ONEE SSO et les 3 applications

---

## 🎯 CE QUI DOIT FONCTIONNER

1. ✅ Connexion sur SSO
2. ✅ Redirection depuis app cliente vers `/connect/authorize`
3. ✅ Page de consentement OIDC
4. ✅ Échange code contre token (`/connect/token`)
5. ✅ Retour vers app avec token
6. ✅ SSO : Pas de nouveau login pour les autres apps

---

## 📋 PRÉREQUIS

### Ports utilisés :
- **ONEE SSO** : `http://localhost:5205`
- **RH Backend** : `http://localhost:5291`
- **RH Frontend** : `http://localhost:5174`
- **TIMS Backend** : `http://localhost:5115`
- **TIMS Frontend** : `http://localhost:5175`
- **EAMS Backend** : `http://localhost:5137`
- **EAMS Frontend** : `http://localhost:5173`

---

## 🚀 ÉTAPE 1 : LANCER LE SSO

### Terminal 1 : SSO
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet run
```

**✅ Vérifier :**
- Le serveur démarre sur `http://localhost:5205`
- Aucune erreur de compilation
- Message "Application started"

**📸 Logs attendus :**
```
== Début Program ==
== Builder créé ==
== Migration ==
== Seed Clients ==
== Seed Roles ==
== Seed Users ==
...
== Avant Run ==
Environment : Development
```

---

## 🧪 TEST MANUEL : PAGE D'AUTORISATION

Avant de tester avec les apps, testons la page `/connect/authorize` directement.

### Ouvrir le navigateur :
```
http://localhost:5205/connect/authorize?client_id=gestion-personnel&redirect_uri=http://localhost:5174/callback&response_type=code&scope=openid%20profile%20email&state=test123
```

### ✅ Résultat attendu :

#### Si NON connecté :
- Redirection automatique vers `/Login`
- URL contient `return_url` avec tous les paramètres OIDC
- Login normal

#### Après connexion :
- Voir la page de consentement
- Titre : "Autorisation requise"
- Badge : "Connecté en tant que votre-email@example.com"
- Icône RH (users)
- Nom : "Gestion Personnel"
- Liste des permissions :
  - ✓ Authentification unique (SSO)
  - ✓ Accès à votre profil
  - ✓ Accès à votre adresse email
- Boutons "Refuser" et "Autoriser"

### Tester "Autoriser" :
- Clic sur "Autoriser"
- Redirection vers : `http://localhost:5174/callback?code=XXXXXXX&state=test123`
- **Note** : L'app RH n'est pas encore lancée, donc erreur 404 normal

### Tester "Refuser" :
- Clic sur "Refuser"
- Redirection vers : `http://localhost:5174/callback?error=access_denied&state=test123`

---

## 🚀 ÉTAPE 2 : LANCER GESTION PERSONNEL (RH)

### Terminal 2 : RH Backend
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\GestionPersonnel\GestionPersonnel.API
dotnet run
```

**✅ Vérifier :**
- Serveur démarre sur `http://localhost:5291`
- Message "Application started"

### Terminal 3 : RH Frontend
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\GestionPersonnel\gestion-personnel-frontend
npm run dev
```

**✅ Vérifier :**
- Vite démarre sur `http://localhost:5174`
- Message "ready in XXXms"

---

## 🧪 TEST COMPLET : FLUX SSO AVEC RH

### 1. Ouvrir RH dans le navigateur
```
http://localhost:5174
```

### 2. Cliquer sur "Se connecter avec ONEE SSO"

**✅ Vérifier :**
- Redirection vers `http://localhost:5205/connect/authorize?...`
- Page de consentement s'affiche (si déjà connecté à SSO)
- OU page Login s'affiche (si non connecté)

### 3. Si Login : Se connecter

**Utilisateur de test :**
- Email : `admin@onee.ma`
- Mot de passe : `Admin@123`

**✅ Vérifier :**
- Authentification réussie
- Redirection automatique vers page consentement

### 4. Consentement : Cliquer "Autoriser"

**✅ Vérifier :**
- Redirection vers : `http://localhost:5174/callback?code=XXXXXXX&state=...`
- Le frontend RH intercepte cette URL

**📸 Dans le terminal SSO, vérifier les logs :**
```
Token request received: grant_type=authorization_code, code=XXXXXXXX, client_id=gestion-personnel
Token exchange successful for client_id=gestion-personnel
```

### 5. RH échange le code contre un token

**✅ Vérifier :**
- RH appelle `POST /connect/token`
- Reçoit le JWT token
- Stocke le token
- Affiche l'interface RH avec utilisateur connecté

**📸 Dans le terminal RH Backend :**
```
Successfully obtained token from SSO
User authenticated: admin@onee.ma
```

### 6. RH affiche le Dashboard

**✅ Vérifier :**
- Nom utilisateur affiché
- Email affiché
- Menu accessible
- Pas d'erreur dans la console

---

## 🚀 ÉTAPE 3 : TESTER LE SSO (SINGLE SIGN-ON)

Maintenant, testons que l'utilisateur N'A PAS besoin de se reconnecter pour accéder aux autres apps.

### Lancer TIMS

#### Terminal 4 : TIMS Backend
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\TIMS\TIMS.API
dotnet run
```

#### Terminal 5 : TIMS Frontend
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\TIMS\tims-frontend
npm run dev
```

### Ouvrir TIMS dans le navigateur
```
http://localhost:5175
```

### Cliquer sur "Se connecter avec ONEE SSO"

**✅ RÉSULTAT ATTENDU (SSO) :**
- ❌ **PAS** de page Login !
- ✅ Page de consentement directement (car déjà authentifié sur SSO)
- ✅ Cliquer "Autoriser"
- ✅ Redirection vers TIMS avec code
- ✅ TIMS obtient le token
- ✅ TIMS affiche le dashboard
- ✅ **AUCUNE SAISIE de mot de passe demandée !**

**🎉 SI CELA FONCTIONNE = SSO RÉUSSI !**

---

## 🚀 ÉTAPE 4 : TESTER AVEC EAMS

### Lancer EAMS

#### Terminal 6 : EAMS Backend
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\EAMS\EAMS.API
dotnet run
```

#### Terminal 7 : EAMS Frontend
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\EAMS\eams-frontend
npm run dev
```

### Ouvrir EAMS dans le navigateur
```
http://localhost:5173
```

### Cliquer sur "Se connecter avec ONEE SSO"

**✅ RÉSULTAT ATTENDU (SSO) :**
- ❌ **PAS** de page Login !
- ✅ Page de consentement directement
- ✅ Accès à EAMS sans nouveau mot de passe

**🎉 SI CELA FONCTIONNE = SSO RÉUSSI POUR LES 3 APPS !**

---

## 🧪 TEST SUPPLÉMENTAIRE : LOGOUT GLOBAL

### Depuis n'importe quelle app, se déconnecter

### Aller sur SSO :
```
http://localhost:5205/Logout
```

**✅ Vérifier :**
- Message "Vous êtes déconnecté"
- Session supprimée

### Retourner sur RH
```
http://localhost:5174
```

**✅ Vérifier :**
- Utilisateur déconnecté
- Clic sur "Se connecter" demande à nouveau le Login

### Retourner sur TIMS
```
http://localhost:5175
```

**✅ Vérifier :**
- Utilisateur déconnecté
- Clic sur "Se connecter" demande à nouveau le Login

**🎉 SI CELA FONCTIONNE = LOGOUT GLOBAL RÉUSSI !**

---

## ❌ PROBLÈMES POSSIBLES

### Problème 1 : 404 sur `/connect/authorize`
**Cause :** Razor Pages non activées  
**Solution :** Vérifier `Program.cs` contient `app.MapRazorPages()`

### Problème 2 : "Authorization code not found"
**Cause :** Session non partagée entre requêtes  
**Solution :** Vérifier cookies de session (SameSite, Secure)

### Problème 3 : Redirection infinie
**Cause :** Loop entre app et SSO  
**Solution :** Vérifier `redirect_uri` est correcte

### Problème 4 : CORS error
**Cause :** Frontend appelle SSO en AJAX  
**Solution :** Utiliser redirection navigateur (pas fetch)

### Problème 5 : Page Login au lieu de consentement (2e app)
**Cause :** Session SSO perdue  
**Solution :** Vérifier cookies partagés entre domaines

---

## 📊 CHECKLIST FINALE

### MVP Utilisateur - Phase 1
- [ ] SSO démarre sans erreur
- [ ] Page Login accessible
- [ ] Connexion réussie avec `admin@onee.ma`
- [ ] Dashboard SSO affiche profil complet
- [ ] Redirection depuis RH vers `/connect/authorize`
- [ ] Page consentement s'affiche correctement
- [ ] Bouton "Autoriser" génère un code
- [ ] Endpoint `/connect/token` échange le code
- [ ] RH reçoit le token JWT
- [ ] RH affiche le dashboard utilisateur
- [ ] TIMS accède sans nouveau login (SSO)
- [ ] EAMS accède sans nouveau login (SSO)
- [ ] Logout global fonctionne

---

## 🎓 POUR LA SOUTENANCE

### Démonstration recommandée :

**1. Introduction (30s)**
- "J'ai développé un serveur SSO pour ONEE avec interface web complète"

**2. Architecture (1min)**
- Montrer schéma : 1 SSO + 3 applications
- Expliquer le protocole OIDC/OAuth2

**3. Démonstration live (3min)**
- Ouvrir RH → Clic "Se connecter avec SSO"
- Login une seule fois
- Autorisation
- Accès à RH
- **Ouvrir TIMS → Accès IMMÉDIAT sans login**
- **Ouvrir EAMS → Accès IMMÉDIAT sans login**
- Montrer Dashboard SSO avec profil
- Logout global

**4. Code technique (2min)**
- Montrer page `/connect/authorize` (consentement)
- Montrer endpoint `/connect/token` (échange)
- Montrer Design System ONEE

**5. Conclusion (30s)**
- SSO opérationnel pour 3 applications
- Interface professionnelle
- Sécurité (JWT, sessions, HTTPS)

---

## 📞 COMMANDE RAPIDE : LANCER TOUT

Créer un script PowerShell `LANCER_TOUS.ps1` :

```powershell
# Terminal 1 : SSO
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API'; dotnet run"

# Attendre 5 secondes
Start-Sleep -Seconds 5

# Terminal 2 : RH Backend
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'c:\Users\XPS\source\repos\ONEE.SSO\clients\GestionPersonnel\GestionPersonnel.API'; dotnet run"

# Terminal 3 : RH Frontend
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'c:\Users\XPS\source\repos\ONEE.SSO\clients\GestionPersonnel\gestion-personnel-frontend'; npm run dev"

# Terminal 4 : TIMS Backend
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'c:\Users\XPS\source\repos\ONEE.SSO\clients\TIMS\TIMS.API'; dotnet run"

# Terminal 5 : TIMS Frontend
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'c:\Users\XPS\source\repos\ONEE.SSO\clients\TIMS\tims-frontend'; npm run dev"

# Terminal 6 : EAMS Backend
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'c:\Users\XPS\source\repos\ONEE.SSO\clients\EAMS\EAMS.API'; dotnet run"

# Terminal 7 : EAMS Frontend
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'c:\Users\XPS\source\repos\ONEE.SSO\clients\EAMS\eams-frontend'; npm run dev"
```

---

**🎯 OBJECTIF : Valider que le SSO fonctionne parfaitement entre les 3 applications !**
