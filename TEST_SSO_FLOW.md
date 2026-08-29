# TEST SSO FLOW - OIDC Implementation

## ✅ CHANGEMENTS EFFECTUÉS

### 1. Ajout de la génération d'`id_token` (OIDC Standard)

**Fichiers modifiés:**
- `src/ONEE.SSO.Application/Interfaces/IJwtService.cs` - Ajout méthode `GenerateIdToken()`
- `src/ONEE.SSO.Infrastructure/Security/JwtService.cs` - Implémentation de `GenerateIdToken()`
- `src/ONEE.SSO.API/Controllers/ConnectController.cs` - Génération et retour des 2 tokens

**Pourquoi ?**
- La spec OIDC (OpenID Connect) **EXIGE** 2 tokens :
  - **`access_token`**: Pour accéder aux APIs (contient roles + permissions)
  - **`id_token`**: Pour identifier l'utilisateur (contient sub, email, name)
- La bibliothèque `oidc-client-ts` utilisée par le frontend RH rejette les réponses qui ne contiennent que `access_token`

### 2. Structure des tokens

**access_token (API Access):**
```json
{
  "sub": "user-id-guid",
  "email": "admin@onee.ma",
  "jti": "unique-token-id",
  "role": ["Admin", "Manager"],
  "permission": ["users.read", "users.write", ...]
}
```

**id_token (User Identity - OIDC):**
```json
{
  "sub": "user-id-guid",
  "email": "admin@onee.ma",
  "name": "John Doe",
  "email_verified": true,
  "aud": "gestion-personnel",
  "iss": "https://localhost:5205",
  "exp": 1234567890
}
```

## 🧪 COMMANDES DE TEST

### Étape 1: Démarrer le SSO
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet run
```
**Attendre:** `Now listening on: https://localhost:5205`

### Étape 2: Démarrer le Backend RH (dans un nouveau terminal)
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\gestion-personnel\backend\ONEE.GestionPersonnel.API
dotnet run
```
**Attendre:** `Now listening on: http://localhost:5291`

### Étape 3: Démarrer le Frontend RH (dans un nouveau terminal)
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\gestion-personnel\frontend
npm run dev
```
**Attendre:** `Local: http://localhost:5173`

## 🔍 FLOW COMPLET À TESTER

1. **Ouvrir:** http://localhost:5173 dans le navigateur
2. **Cliquer:** Bouton "Se connecter"
3. **Redirection:** Vers SSO Login (https://localhost:5205/Login)
4. **Connexion:** Email: `admin@onee.ma`, Password: `Admin@123`
5. **Consentement:** Page "Autoriser l'accès" s'affiche
6. **Autoriser:** Cliquer "Autoriser"
7. **Redirection:** Vers frontend RH avec code d'autorisation
8. **Token Exchange:** Frontend échange le code contre les tokens
9. **✅ SUCCÈS:** Dashboard RH s'affiche avec les données

## 📋 LOGS À VÉRIFIER

### Terminal SSO (ONEE.SSO.API):
```
[LOGIN SUCCESS] Redirecting to: /connect/authorize?...
[AUTHORIZE] Generated authorization code: xxxxx (length=43)
✅ Stored authorization code: xxxxx for client: gestion-personnel
=== TOKEN ENDPOINT CALLED ===
✅ Consumed authorization code: xxxxx for client: gestion-personnel
✅ Generated access_token and id_token for user: admin@onee.ma, client: gestion-personnel
Token exchange successful for client_id=gestion-personnel
```

### Terminal Backend RH:
```
✅ SSO Token Validated - User: (ID: xxx)
Request finished HTTP/1.1 GET http://localhost:5291/api/dashboard - 200 OK
```

### Navigateur (Console DevTools):
```
✅ Tokens received successfully
✅ User profile loaded
✅ Dashboard data loaded
```

## ❌ PROBLÈME PRÉCÉDENT

**Erreur:** `InvalidTokenError: invalid token specified: missing part #2`

**Cause:** Le endpoint `/connect/token` retournait uniquement `access_token` sans `id_token`

**Solution:** Ajout de la génération et du retour d'`id_token` (conformité OIDC)

## 🎯 PROCHAINES ÉTAPES

1. ✅ **Test complet du flow SSO** avec les 3 commandes ci-dessus
2. **Vérifier** que le dashboard RH s'affiche correctement
3. **Tester** la déconnexion et la reconnexion
4. **Tester** avec TIMS (port 5175) et EAMS (port 5173)
5. **Phase 2:** Implémenter les pages d'administration SSO:
   - Dashboard SSO (`/Dashboard`)
   - Gestion des utilisateurs (`/Users`)
   - Gestion des rôles (`/Roles`)
   - Gestion des applications clientes (`/ClientApplications`)
   - Sessions actives (`/Sessions`)
   - Audit logs (`/AuditLogs`)

## 📞 CREDENTIALS DE TEST

- **Email:** admin@onee.ma
- **Password:** Admin@123
- **Roles:** Admin, Manager
- **Permissions:** Toutes (12 permissions)
