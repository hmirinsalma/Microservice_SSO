# 🔒 Sécurité

## Configuration avant déploiement

### ⚠️ IMPORTANT : Avant de déployer en production

1. **Changer tous les secrets** dans les fichiers `appsettings.json` :
   - JWT Secret (minimum 32 caractères)
   - Client Secrets OIDC
   - Clés de chiffrement

2. **Utiliser des variables d'environnement** pour les secrets sensibles

3. **Activer HTTPS** en production

4. **Configurer CORS** correctement (pas de wildcard `*`)

5. **Changer le mot de passe admin par défaut** (`Admin@123`)

## Mots de passe de développement

Les fichiers suivants contiennent des mots de passe/secrets de **DÉVELOPPEMENT UNIQUEMENT** :

- `src/ONEE.SSO.API/appsettings.json` → JWT Secret
- `src/ONEE.SSO.Infrastructure/Persistence/Seed/UsersSeeder.cs` → Mot de passe admin par défaut
- `clients/eams/frontend/src/pages/Login.tsx` → Credentials pré-remplis pour dev

**Ces valeurs DOIVENT être changées avant tout déploiement en production.**

## Chaînes de connexion

Les chaînes de connexion dans les `appsettings.json` utilisent `Integrated Security=True` (authentification Windows), donc pas de mot de passe en clair.

Pour un environnement de production, utiliser :
- Azure SQL avec Managed Identity
- Ou des variables d'environnement sécurisées

## Bonnes pratiques implémentées

✅ **Hashage des mots de passe** avec BCrypt  
✅ **JWT** avec signature HMAC-SHA256  
✅ **PKCE** (Proof Key for Code Exchange) activé  
✅ **Protection contre les attaques par force brute** (verrouillage automatique après 5 tentatives)  
✅ **Rotation des Refresh Tokens**  
✅ **Audit logs** complets  
✅ **CORS** configuré  
✅ **Validation des entrées** côté client et serveur  

## Signaler une vulnérabilité

Si vous découvrez une vulnérabilité de sécurité, veuillez **NE PAS** créer une issue publique.

Contactez-nous directement par email : **security@onee.ma** (ou créez une Security Advisory privée sur GitHub)

## Configuration recommandée en production

### 1. Variables d'environnement

```bash
export JWT_SECRET="votre-secret-super-securise-32-caracteres-minimum"
export RH_CLIENT_SECRET="secret-rh-client"
export TIMS_CLIENT_SECRET="secret-tims-client"
export EAMS_CLIENT_SECRET="secret-eams-client"
```

### 2. appsettings.Production.json

```json
{
  "Jwt": {
    "Secret": "${JWT_SECRET}",
    "Issuer": "https://sso.onee.ma",
    "Audience": "https://sso.onee.ma"
  },
  "Database": {
    "ConnectionString": "${DATABASE_CONNECTION_STRING}"
  }
}
```

### 3. HTTPS obligatoire

```csharp
app.UseHttpsRedirection();
app.UseHsts();
```

### 4. Headers de sécurité

Ajouter :
- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `Content-Security-Policy`
- `Strict-Transport-Security`

## Checklist avant production

- [ ] Changer JWT Secret
- [ ] Changer tous les Client Secrets OIDC
- [ ] Changer le mot de passe admin par défaut
- [ ] Activer HTTPS
- [ ] Configurer CORS avec des domaines spécifiques
- [ ] Utiliser des variables d'environnement pour les secrets
- [ ] Activer les headers de sécurité
- [ ] Configurer les logs de production
- [ ] Tester les scénarios de sécurité
- [ ] Effectuer un audit de sécurité
