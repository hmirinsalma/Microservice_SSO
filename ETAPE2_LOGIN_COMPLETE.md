# ✅ ÉTAPE 2 TERMINÉE : PAGE DE LOGIN

## 📋 CE QUI A ÉTÉ CRÉÉ

### 1️⃣ Page de Login complète
- ✅ `Login.cshtml` - Interface de connexion professionnelle
- ✅ `Login.cshtml.cs` - Code-behind avec appel API
- ✅ Formulaire avec validation
- ✅ Toggle password visibility
- ✅ Messages d'erreur clairs
- ✅ Loading state pendant l'authentification
- ✅ Gestion des comptes verrouillés/désactivés

### 2️⃣ Page Dashboard temporaire
- ✅ `Dashboard.cshtml` - Page après connexion
- ✅ `Dashboard.cshtml.cs` - Vérification de session
- ✅ Affichage email et token
- ✅ Bouton de déconnexion

### 3️⃣ Configuration
- ✅ Ajout de `HttpClientFactory` dans Program.cs
- ✅ Ajout des Sessions pour stocker les tokens
- ✅ Configuration `BaseUrl` dans appsettings.json
- ✅ Middleware Session activé

---

## 🎨 FONCTIONNALITÉS DE LA PAGE LOGIN

### Design
- ✅ Logo ONEE avec icône bouclier
- ✅ Titre et sous-titre
- ✅ Card blanche centrée
- ✅ Background gradient
- ✅ Responsive

### Formulaire
- ✅ Champ Email avec validation
- ✅ Champ Mot de passe avec toggle visibility
- ✅ Checkbox "Se souvenir de moi"
- ✅ Lien "Mot de passe oublié ?"
- ✅ Bouton de connexion avec loading

### Intégration API
- ✅ Appel à `/api/Auth/login`
- ✅ Gestion des erreurs :
  - Email/mot de passe incorrect
  - Compte verrouillé (après 5 tentatives)
  - Compte désactivé
  - Erreur serveur
- ✅ Stockage du token dans la session
- ✅ Redirection vers Dashboard après connexion

### Paramètres URL
- ✅ `?client_name=` - Affiche "Connexion à [nom]"
- ✅ `?return_url=` - Redirige après connexion
- ✅ Support du flow OIDC

---

## 🧪 TESTS À EFFECTUER

### Étape 1 : Arrêter le serveur SSO actuel

**Dans la fenêtre PowerShell du SSO** :
```
Ctrl+C
```

---

### Étape 2 : Rebuild le projet

```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet build
```

✅ **Attendu** : Build réussit sans erreur

---

### Étape 3 : Lancer le serveur

```powershell
dotnet run
```

✅ **Attendu** : 
```
Now listening on: http://localhost:5205
```

---

### Étape 4 : Tester la page de Login

#### Test 1 : Accès à la page
Ouvrir : **http://localhost:5205/Login**

✅ **Attendu** :
- Card blanche centrée
- Logo bouclier bleu ONEE
- Titre "ONEE SSO"
- Sous-titre "Authentification Unique"
- Formulaire avec Email et Mot de passe
- Checkbox "Se souvenir de moi"
- Lien "Mot de passe oublié ?"
- Bouton "Se connecter"
- Lien "Documentation API"
- Footer ONEE

---

#### Test 2 : Validation du formulaire
1. Cliquer sur **"Se connecter"** sans remplir les champs

✅ **Attendu** : Messages d'erreur sous les champs

---

#### Test 3 : Toggle mot de passe
1. Saisir un mot de passe
2. Cliquer sur l'icône œil

✅ **Attendu** : 
- Le mot de passe devient visible
- L'icône change en œil barré
- Clic à nouveau → Le mot de passe est masqué

---

#### Test 4 : Connexion réussie
1. Saisir :
   ```
   Email    : admin@onee.ma
   Password : Admin@123
   ```
2. Cliquer sur **"Se connecter"**

✅ **Attendu** :
- Bouton affiche "Chargement..." avec spinner
- Redirection vers `/Dashboard`
- Dashboard affiche :
  - "Bienvenue !"
  - Alert verte "Authentification réussie !"
  - Email : admin@onee.ma
  - Token d'accès (tronqué)
  - Bouton "Se déconnecter"

---

#### Test 5 : Identifiants incorrects
1. Retourner sur `/Login`
2. Saisir :
   ```
   Email    : test@test.com
   Password : wrongpassword
   ```
3. Cliquer sur **"Se connecter"**

✅ **Attendu** :
- Alert rouge s'affiche
- Message : "Email ou mot de passe incorrect."
- Reste sur la page de login

---

#### Test 6 : Compte verrouillé (simulation)
1. Essayer de se connecter **5 fois** avec un mauvais mot de passe

✅ **Attendu** :
- Alert rouge
- Message : "Votre compte a été verrouillé suite à plusieurs tentatives de connexion échouées."

---

#### Test 7 : Paramètre client_name
Ouvrir : **http://localhost:5205/Login?client_name=Gestion%20Personnel**

✅ **Attendu** :
- Sous-titre affiche : "Connexion à **Gestion Personnel**"

---

### Étape 5 : Tester la page d'accueil

Ouvrir : **http://localhost:5205**

✅ **Attendu** :
- Page Index s'affiche
- Bouton "Se connecter" redirige vers `/Login`

---

### Étape 6 : Tester Swagger

Ouvrir : **http://localhost:5205/swagger**

✅ **Attendu** : Swagger fonctionne toujours

---

## 📸 CE QUE VOUS DEVRIEZ VOIR

### Page Login
```
┌─────────────────────────────────────────┐
│           [Logo bouclier bleu]          │
│              ONEE SSO                   │
│        Authentification Unique          │
│                                         │
│  ┌──────────────────────────────────┐  │
│  │ ADRESSE EMAIL                    │  │
│  │ votre@email.ma                   │  │
│  └──────────────────────────────────┘  │
│                                         │
│  ┌──────────────────────────────────┐  │
│  │ MOT DE PASSE                 [👁] │  │
│  │ ••••••••                         │  │
│  └──────────────────────────────────┘  │
│                                         │
│  □ Se souvenir de moi                  │
│                  Mot de passe oublié ?  │
│                                         │
│  ┌──────────────────────────────────┐  │
│  │     [🔓] Se connecter           │  │
│  └──────────────────────────────────┘  │
│                                         │
│             ───── ou ─────             │
│                                         │
│  ┌──────────────────────────────────┐  │
│  │     [💻] Documentation API       │  │
│  └──────────────────────────────────┘  │
│                                         │
│        🔒 Connexion sécurisée          │
│      © 2026 ONEE                       │
└─────────────────────────────────────────┘
```

### Page Dashboard (après connexion)
```
┌─────────────────────────────────────────┐
│       [Logo utilisateur bouclier]       │
│            Bienvenue !                  │
│   Vous êtes connecté au SSO ONEE       │
│                                         │
│  ┌──────────────────────────────────┐  │
│  │ ✅ Authentification réussie !    │  │
│  └──────────────────────────────────┘  │
│                                         │
│  EMAIL CONNECTÉ                        │
│  📧 admin@onee.ma                      │
│                                         │
│  TOKEN D'ACCÈS (TRONQUÉ)               │
│  eyJhbGciOiJIUzI1NiIsInR5cCI6...       │
│                                         │
│  ┌──────────────────────────────────┐  │
│  │  [🚪] Se déconnecter             │  │
│  └──────────────────────────────────┘  │
└─────────────────────────────────────────┘
```

---

## 🎯 PROCHAINE ÉTAPE

Une fois que vous confirmez que l'étape 2 fonctionne, nous passerons à :

**ÉTAPE 3 : PAGE LOGOUT + FORGOT PASSWORD**
- Page de déconnexion
- Suppression de la session
- Page "Mot de passe oublié"
- Page "Réinitialisation de mot de passe"

---

## 🆘 DÉPANNAGE

### Build échoue
```powershell
# Nettoyer et rebuilder
dotnet clean
dotnet build
```

### Erreur "Session not available"
➡️ Vérifier que `app.UseSession()` est dans Program.cs **avant** `app.UseAuthentication()`

### Page Login affiche erreur 500
➡️ Vérifier les logs dans la console PowerShell

### API /api/Auth/login ne répond pas
➡️ Vérifier que le serveur SSO est bien lancé sur le port 5205

### Token non stocké dans la session
➡️ Vérifier que `HttpContext.Session.SetString()` est bien appelé

---

**✅ ÉTAPE 2 COMPLÈTE - TESTEZ ET CONFIRMEZ AVANT DE CONTINUER !**

**Pour tester rapidement** :
```powershell
# Arrêter le serveur (Ctrl+C dans le terminal)
dotnet run
# Ouvrir http://localhost:5205/Login
# Se connecter avec admin@onee.ma / Admin@123
```
