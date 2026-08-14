# 🧪 Guide de Tests Sprint 3 - Sécurité Avancée

## 🎯 Objectif

Tester les nouvelles fonctionnalités de sécurité implémentées dans Sprint 3 :
- Forgot Password
- Reset Password
- Change Password
- Account Lockout (Blocage automatique)
- Admin Unlock

---

## 📋 Prérequis

1. **Lancer l'API** :
```bash
dotnet run --project src/ONEE.SSO.API
```

2. **Ouvrir Swagger** :
```
http://localhost:5205/swagger
```

3. **Utilisateur de test** :
- Username: `admin`
- Password: `Admin@123`
- Role: SuperAdmin

---

## ✅ Scénario de Test 1 : Forgot Password

### Étape 1 : Demander un reset de mot de passe

**Endpoint** : `POST /api/auth/forgot-password`

**Request Body** :
```json
{
  "email": "admin@onee.ma"
}
```

**Résultat attendu** :
- Status: `200 OK`
- Message: "If the email exists, a password reset link has been sent"
- Un token est généré dans la base de données (table Users, champ PasswordResetToken)

**Vérification SQL** :
```sql
SELECT Username, Email, PasswordResetToken, PasswordResetTokenExpiresAt 
FROM Users 
WHERE Email = 'admin@onee.ma';
```

Vous devriez voir :
- `PasswordResetToken` : une longue chaîne aléatoire
- `PasswordResetTokenExpiresAt` : date/heure actuelle + 1 heure

---

## ✅ Scénario de Test 2 : Reset Password

### Étape 1 : Récupérer le token depuis la base de données

**SQL** :
```sql
SELECT PasswordResetToken 
FROM Users 
WHERE Email = 'admin@onee.ma';
```

Copiez le token.

### Étape 2 : Réinitialiser le mot de passe

**Endpoint** : `POST /api/auth/reset-password`

**Request Body** :
```json
{
  "token": "COLLEZ_LE_TOKEN_ICI",
  "newPassword": "NewAdmin@456"
}
```

**Résultat attendu** :
- Status: `200 OK`
- Message: "Password has been reset successfully"
- Le mot de passe est changé
- Le token est invalidé (PasswordResetToken = null)
- Toutes les sessions sont révoquées
- IsLocked = false (déblocage automatique si le compte était bloqué)

**Vérification** :
Testez le login avec le nouveau mot de passe :

**Endpoint** : `POST /api/auth/login`
```json
{
  "usernameOrEmail": "admin",
  "password": "NewAdmin@456"
}
```

✅ Le login doit réussir.

---

## ✅ Scénario de Test 3 : Change Password (Utilisateur Authentifié)

### Étape 1 : Se connecter

**Endpoint** : `POST /api/auth/login`
```json
{
  "usernameOrEmail": "admin",
  "password": "NewAdmin@456"
}
```

Copiez le `accessToken` de la réponse.

### Étape 2 : Autoriser les requêtes

Dans Swagger :
1. Cliquez sur le bouton **Authorize** (en haut à droite)
2. Entrez : `Bearer VOTRE_ACCESS_TOKEN`
3. Cliquez sur **Authorize**

### Étape 3 : Changer le mot de passe

**Endpoint** : `POST /api/auth/change-password`

**Request Body** :
```json
{
  "currentPassword": "NewAdmin@456",
  "newPassword": "Admin@789"
}
```

**Résultat attendu** :
- Status: `200 OK`
- Message: "Password changed successfully"
- Toutes les sessions SAUF la courante sont révoquées

### Étape 4 : Vérifier le nouveau mot de passe

Déconnectez-vous et reconnectez-vous avec le nouveau mot de passe :

**Endpoint** : `POST /api/auth/login`
```json
{
  "usernameOrEmail": "admin",
  "password": "Admin@789"
}
```

✅ Le login doit réussir.

---

## ✅ Scénario de Test 4 : Account Lockout (Blocage Automatique)

### Objectif
Tester le blocage automatique après 5 tentatives échouées.

### Étape 1 : Créer un utilisateur test

**Endpoint** : `POST /api/users` (authentifié)
```json
{
  "username": "testuser",
  "email": "testuser@onee.ma",
  "firstName": "Test",
  "lastName": "User",
  "password": "Test@123"
}
```

### Étape 2 : Faire 5 tentatives avec un mauvais mot de passe

**Endpoint** : `POST /api/auth/login`

**Tentative 1-5** (répétez 5 fois) :
```json
{
  "usernameOrEmail": "testuser",
  "password": "MAUVAIS_MOT_DE_PASSE"
}
```

**Résultats attendus** :
- Tentatives 1-4 : Status `401 Unauthorized`, message "Invalid credentials"
- Tentative 5 : Status `401 Unauthorized`, message "Invalid credentials"

### Étape 3 : Vérifier le blocage

**Tentative 6** (avec le BON mot de passe) :
```json
{
  "usernameOrEmail": "testuser",
  "password": "Test@123"
}
```

**Résultat attendu** :
- Status: `403 Forbidden`
- Message: "Account is locked due to multiple failed login attempts. Please contact support."

**Vérification SQL** :
```sql
SELECT Username, FailedLoginAttempts, IsLocked, LockedAt 
FROM Users 
WHERE Username = 'testuser';
```

Vous devriez voir :
- `FailedLoginAttempts` : 5
- `IsLocked` : 1 (true)
- `LockedAt` : date/heure du blocage

---

## ✅ Scénario de Test 5 : Admin Unlock (Déblocage)

### Étape 1 : Se connecter en tant qu'Admin

**Endpoint** : `POST /api/auth/login`
```json
{
  "usernameOrEmail": "admin",
  "password": "Admin@789"
}
```

Copiez le `accessToken` et autorisez dans Swagger.

### Étape 2 : Récupérer l'ID de l'utilisateur bloqué

**Endpoint** : `GET /api/users` (avec filtres)

Cherchez l'utilisateur "testuser" et notez son `id`.

### Étape 3 : Débloquer le compte

**Endpoint** : `POST /api/users/{id}/unlock`

Remplacez `{id}` par l'ID de testuser.

**Résultat attendu** :
- Status: `200 OK`
- Message: "User unlocked successfully"

**Vérification SQL** :
```sql
SELECT Username, FailedLoginAttempts, IsLocked, LockedAt 
FROM Users 
WHERE Username = 'testuser';
```

Vous devriez voir :
- `FailedLoginAttempts` : 0
- `IsLocked` : 0 (false)
- `LockedAt` : NULL

### Étape 4 : Vérifier que le login fonctionne à nouveau

**Endpoint** : `POST /api/auth/login`
```json
{
  "usernameOrEmail": "testuser",
  "password": "Test@123"
}
```

✅ Le login doit réussir.

---

## ✅ Scénario de Test 6 : Validation de Complexité

### Test 1 : Mot de passe trop court

**Endpoint** : `POST /api/auth/reset-password`
```json
{
  "token": "UN_TOKEN_VALIDE",
  "newPassword": "Short1!"
}
```

**Résultat attendu** :
- Status: `400 Bad Request`
- Message: "Password must be at least 8 characters long"

### Test 2 : Pas de majuscule

```json
{
  "token": "UN_TOKEN_VALIDE",
  "newPassword": "nouppercase123!"
}
```

**Résultat attendu** :
- Status: `400 Bad Request`
- Message: "Password must contain at least one uppercase letter"

### Test 3 : Pas de chiffre

```json
{
  "token": "UN_TOKEN_VALIDE",
  "newPassword": "NoDigits!"
}
```

**Résultat attendu** :
- Status: `400 Bad Request`
- Message: "Password must contain at least one digit"

### Test 4 : Pas de caractère spécial

```json
{
  "token": "UN_TOKEN_VALIDE",
  "newPassword": "NoSpecial123"
}
```

**Résultat attendu** :
- Status: `400 Bad Request`
- Message: "Password must contain at least one special character"

### Test 5 : Mot de passe valide

```json
{
  "token": "UN_TOKEN_VALIDE",
  "newPassword": "ValidPass123!"
}
```

**Résultat attendu** :
- Status: `200 OK`

---

## ✅ Scénario de Test 7 : Audit Logs

Après tous les tests ci-dessus, vérifiez que tous les événements sont enregistrés :

**Endpoint** : `GET /api/auditlogs` (authentifié)

**Événements attendus** :
- `ForgotPasswordRequested`
- `PasswordReset`
- `PasswordChanged`
- `LoginFailed` (x5 pour testuser)
- `AccountLocked`
- `LoginAttemptOnLockedAccount`
- `AccountUnlocked`
- `Login` (après déblocage)

**Vérification SQL** :
```sql
SELECT TOP 50 
    EventType, 
    UserId, 
    Username, 
    Details, 
    IpAddress, 
    Timestamp 
FROM AuditLogs 
ORDER BY Timestamp DESC;
```

---

## 📊 Checklist de Tests

### Forgot Password
- [ ] Forgot password avec email valide → 200 OK
- [ ] Forgot password avec email invalide → 200 OK (comportement anti-énumération)
- [ ] Token généré dans la base de données
- [ ] Token expire dans 1 heure

### Reset Password
- [ ] Reset avec token valide → 200 OK
- [ ] Reset avec token expiré → 400 Bad Request
- [ ] Reset avec token invalide → 400 Bad Request
- [ ] Reset avec mot de passe faible → 400 Bad Request
- [ ] Toutes les sessions révoquées
- [ ] Token invalidé après utilisation
- [ ] Compte débloqué automatiquement

### Change Password
- [ ] Change password authentifié → 200 OK
- [ ] Change password sans authentification → 401 Unauthorized
- [ ] Ancien mot de passe incorrect → 400 Bad Request
- [ ] Nouveau mot de passe faible → 400 Bad Request
- [ ] Sessions révoquées sauf la courante

### Account Lockout
- [ ] 5 tentatives échouées → compte bloqué
- [ ] Tentative avec bon mot de passe après blocage → 403 Forbidden
- [ ] FailedLoginAttempts = 5
- [ ] IsLocked = true
- [ ] LockedAt enregistré

### Admin Unlock
- [ ] Admin peut débloquer → 200 OK
- [ ] Non-admin ne peut pas débloquer → 403 Forbidden
- [ ] Compteur remis à 0
- [ ] IsLocked = false
- [ ] Login fonctionne après déblocage

### Audit Logs
- [ ] Tous les événements enregistrés
- [ ] Username, UserId, IpAddress présents
- [ ] Timestamp correct

---

## 🎯 Résultat Attendu Final

✅ **Toutes les fonctionnalités de Sprint 3 fonctionnent correctement**

- Forgot/Reset/Change Password : opérationnels
- Validation de complexité : stricte et fonctionnelle
- Blocage automatique : après 5 échecs
- Déblocage admin : restreint aux admins
- Audit logging : complet
- Migration : appliquée avec succès

---

## 📝 Notes

Si vous rencontrez des erreurs :
1. Vérifiez les logs de l'API dans le terminal
2. Vérifiez les logs Serilog dans `src/ONEE.SSO.API/Logs/`
3. Vérifiez la base de données SQL Server

**Bon test ! 🚀**
