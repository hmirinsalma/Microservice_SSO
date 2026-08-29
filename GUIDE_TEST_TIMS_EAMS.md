# GUIDE DE TEST - TIMS & EAMS

## 🎯 OBJECTIF
Tester le flow SSO sur les applications **TIMS** et **EAMS** après la validation réussie sur l'application **Gestion Personnel (RH)**.

---

## ✅ PRÉ-REQUIS

Tous les services sont déjà démarrés:
- ✅ SSO Backend (Port 5205)
- ✅ TIMS Frontend (Port 5175)
- ✅ TIMS Backend (Port 5115)
- ✅ EAMS Frontend (Port 5174)
- ✅ EAMS Backend (Port 5137)

---

## 🧪 TEST 1: APPLICATION TIMS

### URL: http://localhost:5175

### Étapes:

#### 1. Ouvrir TIMS
```
http://localhost:5175
```

#### 2. Se Connecter avec SSO
- Chercher le bouton "Se connecter avec SSO" ou "Login with SSO"
- Cliquer dessus

#### 3. Login SSO
- Tu seras redirigé vers: `http://localhost:5205/Login`
- **Email**: `admin@onee.ma`
- **Password**: `Admin@123`
- Cliquer "**Se connecter**"

#### 4. Consentement
- Page: `http://localhost:5205/Connect/Authorize`
- Application demandée: **TIMS**
- Scopes: openid, profile, email, roles
- Cliquer "**Autoriser**"

#### 5. Callback
- Redirection vers: `http://localhost:5175/callback?code=xxx`
- Le frontend TIMS échange le code contre un token
- **Vérifier**: Le dashboard TIMS s'affiche

#### 6. Validation ✅ ou ❌
**Si ça marche**:
- ✅ Dashboard TIMS affiché
- ✅ Dashboard reste stable (pas de retour au login)
- ✅ Menu de navigation fonctionnel

**Si ça ne marche pas**:
- Ouvrir DevTools (F12)
- Onglet "Console"
- Prendre note des erreurs
- Vérifier les logs du backend TIMS (Terminal 13)

---

## 🧪 TEST 2: APPLICATION EAMS

### URL: http://localhost:5174

### Étapes:

#### 1. Ouvrir EAMS
```
http://localhost:5174
```

#### 2. Se Connecter avec SSO
- Chercher le bouton "Se connecter avec SSO" ou "Login with SSO"
- Cliquer dessus

#### 3. Login SSO
- Tu seras redirigé vers: `http://localhost:5205/Login`
- **Email**: `admin@onee.ma`
- **Password**: `Admin@123`
- Cliquer "**Se connecter**"

#### 4. Consentement
- Page: `http://localhost:5205/Connect/Authorize`
- Application demandée: **EAMS**
- Scopes: openid, profile, email, roles
- Cliquer "**Autoriser**"

#### 5. Callback
- Redirection vers: `http://localhost:5174/callback?code=xxx`
- Le frontend EAMS échange le code contre un token
- **Vérifier**: Le dashboard EAMS s'affiche

#### 6. Validation ✅ ou ❌
**Si ça marche**:
- ✅ Dashboard EAMS affiché
- ✅ Dashboard reste stable (pas de retour au login)
- ✅ Menu de navigation fonctionnel

**Si ça ne marche pas**:
- Ouvrir DevTools (F12)
- Onglet "Console"
- Prendre note des erreurs
- Vérifier les logs du backend EAMS (Terminal 15)

---

## 🔍 VÉRIFICATIONS IMPORTANTES

### Dans la Console du Navigateur (F12):
```javascript
// Vérifier que le token est stocké
localStorage.getItem('oidc.user:http://localhost:5205:tims')
// ou
localStorage.getItem('oidc.user:http://localhost:5205:eams')
```

**Résultat attendu**:
```json
{
  "id_token": "eyJ...",
  "access_token": "eyJ...",
  "token_type": "Bearer",
  "expires_at": 1787589172
}
```

### Dans les Logs Backend:
**TIMS (Terminal 13)** ou **EAMS (Terminal 15)**:

**Succès**:
```
✅ Token validé. Claims: [...]
```

**Échec**:
```
❌ Authentification échouée: [message d'erreur]
```

---

## 🐛 PROBLÈMES POTENTIELS

### Problème 1: "client_id not found"
**Cause**: Le client TIMS ou EAMS n'est pas seedé dans la base SSO

**Solution**:
1. Vérifier dans la table `client_applications` du SSO
2. Le seed devrait avoir créé:
   - `client_id: "tims"`
   - `client_id: "eams"`

### Problème 2: "redirect_uri mismatch"
**Cause**: L'URI de callback ne correspond pas à celle configurée

**Solution**:
1. Vérifier dans `client_applications.redirect_uris`:
   - TIMS: `http://localhost:5175/callback`
   - EAMS: `http://localhost:5174/callback`

### Problème 3: "IDX10503" ou "IDX10511"
**Cause**: Secret JWT différent ou kid manquant

**Solution**: Déjà corrigé dans les fichiers aujourd'hui
- Secret unifié: `CHANGE_THIS_TO_A_LONG_SECRET_KEY_AT_LEAST_32_CHARACTERS`
- KeyId ajouté: `onee-sso-key-2024`

### Problème 4: CORS Error
**Cause**: Origin non autorisée

**Solution**:
1. Vérifier `Program.cs` du SSO
2. Les origines doivent inclure:
   - `http://localhost:5173` (RH)
   - `http://localhost:5175` (TIMS)
   - `http://localhost:5174` (EAMS)

---

## 📊 TABLEAU RÉCAPITULATIF DES TESTS

| Application | Port | Status | Dashboard Stable | Logout OK |
|-------------|------|--------|------------------|-----------|
| **RH**      | 5173 | ✅ Testé | ✅ Oui | ⏳ À tester |
| **TIMS**    | 5175 | ⏳ À tester | ? | ? |
| **EAMS**    | 5174 | ⏳ À tester | ? | ? |

---

## 🎯 OBJECTIF FINAL

Avoir les 3 applications fonctionnelles avec SSO:
- [x] Gestion Personnel (RH) - ✅ VALIDÉ
- [ ] TIMS - ⏳ En cours de test
- [ ] EAMS - ⏳ En cours de test

---

## 📝 CHECKLIST DE VALIDATION

### Pour chaque application:
- [ ] Bouton "Se connecter avec SSO" visible
- [ ] Redirection vers SSO
- [ ] Login réussi
- [ ] Page de consentement affichée
- [ ] Autorisation accordée
- [ ] Callback reçu
- [ ] Token stocké dans LocalStorage
- [ ] Dashboard affiché
- [ ] Dashboard reste stable
- [ ] Menu de navigation fonctionnel
- [ ] Logout fonctionne

---

## 🚀 COMMANDES UTILES

### Redémarrer TIMS Backend:
```powershell
# Arrêter
Ctrl+C dans le terminal 13

# Redémarrer
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\tims\backend\TIMS.API
dotnet run
```

### Redémarrer EAMS Backend:
```powershell
# Arrêter
Ctrl+C dans le terminal 15

# Redémarrer
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\eams\backend\ONEE.EAMS.API
dotnet run
```

### Voir les logs en direct:
```powershell
# TIMS Backend: Terminal 13
# EAMS Backend: Terminal 15
# SSO Backend: Terminal 10
```

---

## 💡 CONSEILS

1. **Tester une application à la fois**
2. **Ouvrir DevTools avant de commencer**
3. **Noter les erreurs exactes si problème**
4. **Vérifier les logs backend après chaque tentative**
5. **Se déconnecter complètement entre chaque test** (vider LocalStorage)

---

## 🎓 POUR LA SOUTENANCE

Si les 3 applications fonctionnent:
- **Montrer le dashboard de chaque application**
- **Expliquer qu'avec 1 seul login, on accède aux 3 apps**
- **Montrer le logout centralisé** (se déconnecter d'une app = déconnexion de toutes)

Si seulement RH fonctionne:
- **Expliquer que RH est validé et opérationnel**
- **Les autres apps sont configurées de la même manière**
- **Le principe est identique, seule la configuration client change**

---

**Prêt pour les tests! Bonne chance! 🚀**

---

**Date**: 24 Août 2026  
**Status**: ⏳ En attente des tests TIMS & EAMS
