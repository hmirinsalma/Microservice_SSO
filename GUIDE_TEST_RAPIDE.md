# GUIDE DE TEST RAPIDE - SSO ONEE

## 🚀 DÉMARRAGE RAPIDE (5 minutes)

### Étape 1: Démarrer le SSO
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet run
```
**Attendre**: `Application started` dans la console  
**URL SSO**: http://localhost:5205

---

### Étape 2: Tester l'Interface Admin

#### 2.1 Dashboard
```
http://localhost:5205/Dashboard
```
**Vérifier**:
- ✅ Sidebar navigation visible
- ✅ 4 cartes de statistiques
- ✅ Activité récente
- ✅ Résumé des applications

#### 2.2 Utilisateurs
```
http://localhost:5205/Users/Index
```
**Actions à tester**:
- ✅ Rechercher par email: "admin"
- ✅ Filtrer par rôle: "Admin"
- ✅ Filtrer par statut: "Actif"
- ✅ Voir les 3 utilisateurs seed

#### 2.3 Rôles
```
http://localhost:5205/Roles/Index
```
**Actions à tester**:
- ✅ Cliquer "Créer un rôle"
- ✅ Remplir: Nom="TestRole", Description="Test"
- ✅ Sauvegarder
- ✅ Cliquer "Permissions" sur le nouveau rôle
- ✅ Sélectionner quelques permissions
- ✅ Sauvegarder
- ✅ Supprimer le rôle de test

#### 2.4 Applications
```
http://localhost:5205/ClientApplications
```
**Vérifier**:
- ✅ 3 cartes: Gestion Personnel, TIMS, EAMS
- ✅ Toggle actif/inactif fonctionne
- ✅ Statistiques affichées

#### 2.5 Sessions
```
http://localhost:5205/Sessions
```
**Vérifier**:
- ✅ Cartes statistiques
- ✅ Filtres par application
- ✅ Tableau avec sessions mock
- ✅ Boutons "Révoquer" présents

#### 2.6 Logs d'Audit
```
http://localhost:5205/AuditLogs
```
**Vérifier**:
- ✅ Timeline des actions
- ✅ Filtres (action, entité, date)
- ✅ Détails expandables
- ✅ Couleurs par type d'action

#### 2.7 Paramètres
```
http://localhost:5205/Settings
```
**Vérifier**:
- ✅ 4 onglets: Général, Sécurité, Email, Avancé
- ✅ Formulaires complets
- ✅ Bouton "Enregistrer" sur chaque onglet
- ✅ Zone de danger en bas

**Test de sauvegarde**:
1. Onglet "Général"
2. Changer "Nom de l'organisation"
3. Cliquer "Enregistrer"
4. ✅ Message de succès s'affiche en vert

---

## 🔐 TEST DU FLOW SSO COMPLET

### Étape 3: Démarrer l'Application RH

#### 3.1 Démarrer le Backend RH
```powershell
# Ouvrir un nouveau terminal PowerShell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\gestion-personnel\backend
dotnet run
```
**Attendre**: `Application started`  
**Port**: http://localhost:5291

#### 3.2 Démarrer le Frontend RH
```powershell
# Ouvrir un TROISIÈME terminal PowerShell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\gestion-personnel\frontend
npm run dev
```
**Attendre**: `Local: http://localhost:5173`  
**URL**: http://localhost:5173

---

### Étape 4: Tester le Flow SSO

#### 4.1 Page de Login RH
1. Ouvrir http://localhost:5173
2. **Vérifier**: Page de login RH s'affiche
3. Cliquer sur "Se connecter avec SSO"

#### 4.2 Page de Login SSO
1. **Vérifier**: Redirection vers http://localhost:5205/Login?returnUrl=...
2. **Login**: `admin@onee.ma`
3. **Password**: `Admin@123`
4. Cliquer "Se connecter"

#### 4.3 Page de Consentement
1. **Vérifier**: Page http://localhost:5205/Connect/Authorize
2. **Voir**: 
   - Nom de l'application: "Gestion Personnel"
   - Scopes demandés: openid, profile, email, roles
   - Informations utilisateur
3. Cliquer "Autoriser"

#### 4.4 Dashboard RH
1. **Vérifier**: Redirection vers http://localhost:5173/dashboard
2. **IMPORTANT**: Le dashboard doit rester affiché (ne pas retourner au login)
3. **Vérifier**:
   - ✅ Menu de navigation RH visible
   - ✅ Nom de l'utilisateur affiché
   - ✅ Contenu du dashboard

#### 4.5 Vérifier le Token JWT
1. Ouvrir les DevTools (F12)
2. Onglet "Application" → "Local Storage" → http://localhost:5173
3. **Chercher**: 
   - `oidc.user:http://localhost:5205:gestion-personnel`
4. **Vérifier le contenu**:
   ```json
   {
     "access_token": "eyJ...",
     "id_token": "eyJ...",
     "token_type": "Bearer",
     "expires_at": ...
   }
   ```

#### 4.6 Vérifier la Console Backend RH
Retourner au terminal du backend RH, **vérifier**:
```
✅ Token validated successfully
✅ User: admin@onee.ma
✅ Roles: Admin, User
```

**NE DOIT PAS VOIR**:
```
❌ IDX10517: Signature validation failed. The token's kid is missing
```

#### 4.7 Logout
1. Dans le dashboard RH, cliquer "Se déconnecter"
2. **Vérifier**: Retour à la page de login RH
3. **Vérifier**: Plus de token dans LocalStorage

---

## 🎯 CHECKLIST DE VALIDATION

### Interface Admin SSO
- [ ] Dashboard accessible et statistiques affichées
- [ ] Liste des utilisateurs avec recherche/filtres
- [ ] CRUD des rôles avec permissions
- [ ] Liste des applications clientes
- [ ] Sessions actives affichées
- [ ] Logs d'audit timeline
- [ ] Paramètres avec 4 onglets
- [ ] Navigation fluide dans le menu
- [ ] Design ONEE cohérent
- [ ] Responsive sur mobile

### Flow SSO - Gestion Personnel
- [ ] Redirect vers SSO depuis RH
- [ ] Login avec admin@onee.ma
- [ ] Page de consentement affichée
- [ ] Autorisation réussie
- [ ] Dashboard RH s'affiche
- [ ] Dashboard reste stable (pas de logout auto)
- [ ] Token JWT stocké dans LocalStorage
- [ ] Backend valide le token (avec kid)
- [ ] Pas d'erreur IDX10517
- [ ] Logout fonctionne

### Console Logs à Vérifier
#### SSO Backend (5205):
```
✅ [LOGIN SUCCESS] Redirecting to: /connect/authorize?...
✅ [AUTHORIZE] Generated authorization code: ...
✅ [TOKEN] ✅ Generated access_token and id_token
```

#### RH Backend (5291):
```
✅ Token validated successfully
✅ User authenticated: admin@onee.ma
```

#### RH Frontend (5173):
```
✅ User loaded from OIDC
✅ Access token: eyJ...
```

---

## ⚠️ PROBLÈMES COURANTS

### Problème 1: "IDX10517: kid is missing"
**Cause**: Version ancienne du JwtService sans le fix  
**Solution**: 
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet build
dotnet run
```

### Problème 2: Dashboard RH retourne au login
**Cause**: `oidc-client-ts` appelle automatiquement logout  
**Solution**: Vérifier `authConfig.js`:
```javascript
automaticSilentRenew: false,
loadUserInfo: false
```

### Problème 3: CORS error
**Vérifier Program.cs**:
```csharp
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.WithOrigins(
            "http://localhost:5173",  // RH
            "http://localhost:5175",  // TIMS
            "http://localhost:5174"   // EAMS
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
```

### Problème 4: "Client not found"
**Vérifier**: Les clients sont bien seedés dans la base
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet run
```
Les logs doivent afficher:
```
== Seed Clients ==
Seeded client: gestion-personnel
Seeded client: tims
Seeded client: eams
```

---

## 🎓 DÉMO POUR LA SOUTENANCE

### Scénario Complet (5 minutes)

#### 1. Montrer l'Interface Admin (2 min)
- Dashboard: "Voici la vue d'ensemble du système SSO"
- Utilisateurs: "On peut gérer tous les utilisateurs depuis ici"
- Rôles: "Gestion fine des permissions par rôle"
- Applications: "Les 3 applications clientes configurées"

#### 2. Montrer le Flow SSO (2 min)
- Ouvrir RH: "Un utilisateur veut accéder à l'app RH"
- Login SSO: "Il se connecte une seule fois"
- Consentement: "Il autorise l'accès"
- Dashboard: "Il accède directement à son dashboard"

#### 3. Montrer la Centralisation (1 min)
- "Avec ce token, l'utilisateur peut accéder aux 3 apps"
- "Un seul logout déconnecte de toutes les apps"
- "Administration centralisée des droits"

---

## 📊 RÉSULTATS ATTENDUS

### Succès ✅
- Toutes les pages admin accessibles
- Navigation fluide
- Flow SSO complet fonctionnel
- Dashboard RH stable après login
- Token JWT validé avec kid
- Logout centralisé fonctionne

### Temps Estimé
- Test interface admin: 10 minutes
- Test flow SSO: 5 minutes
- **Total**: 15 minutes

---

## 🏁 VALIDATION FINALE

Une fois tous les tests passés, le projet est **prêt pour la soutenance**!

**Checklist finale**:
- [ ] SSO démarre sans erreur
- [ ] Interface admin complète
- [ ] Flow SSO sur au moins 1 application (RH)
- [ ] Pas d'erreur JWT dans les logs
- [ ] Design professionnel et cohérent
- [ ] Documentation prête

**Prochaine étape**: Préparer la présentation PowerPoint! 📊

---

**Date de validation**: ___________  
**Testé par**: ___________  
**Statut**: [ ] VALIDÉ  [ ] À CORRIGER
