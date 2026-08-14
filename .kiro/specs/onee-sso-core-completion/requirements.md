# Requirements Document

## Introduction

Ce document décrit les exigences fonctionnelles et de sécurité pour finaliser le microservice ONEE.SSO, un fournisseur d'identité centralisé (Identity Provider) basé sur ASP.NET Core 9 et Clean Architecture. L'objectif est de compléter le cycle d'authentification complet (login, refresh token, logout, sessions), d'implémenter l'intégration SSO avec trois applications clientes existantes, de mettre en place la journalisation d'audit, la gestion avancée de la sécurité des mots de passe, et la documentation finale.

La base de données, les entités, les repositories et les services CRUD de base sont déjà validés. Ce spec couvre exclusivement ce qui reste à implémenter pour atteindre un SSO production-ready.

---

## Glossary

- **SSO_Server** : Le microservice ONEE.SSO (ASP.NET Core 9, Identity Provider central)
- **Auth_Service** : Service d'application gérant le cycle d'authentification (login, logout, refresh, sessions)
- **JWT_Service** : Service responsable de la génération et validation des tokens JWT
- **RefreshToken_Service** : Service responsable de la rotation, émission et révocation des refresh tokens
- **Session_Service** : Service responsable du tracking des sessions utilisateur
- **AuditLog_Service** : Service responsable de la journalisation des événements de sécurité et métier
- **Password_Service** : Service responsable des opérations de gestion du mot de passe (reset, change, vérification)
- **ClientApp_Service** : Service responsable de la validation et gestion des applications clientes enregistrées
- **User** : Entité représentant un utilisateur du système, stockée dans la table `Users`
- **ClientApplication** : Entité représentant une application cliente enregistrée (ClientId, ClientSecret, RedirectUri, Scopes)
- **AccessToken** : Jeton JWT à courte durée de vie (15 min), signé par le SSO_Server, utilisé pour accéder aux ressources protégées
- **RefreshToken** : Jeton opaque à longue durée de vie (30 jours) permettant l'obtention d'un nouvel AccessToken sans re-authentification
- **UserSession** : Enregistrement d'une session active d'un User, liant un appareil, une IP et un RefreshToken
- **AuditLog** : Enregistrement immuable d'un événement de sécurité ou d'une opération métier importante
- **PKCE** : Proof Key for Code Exchange — extension OAuth2 empêchant l'interception du code d'autorisation
- **FailedLoginAttempts** : Compteur d'échecs consécutifs de login pour un User donné
- **LockoutPolicy** : Politique de blocage automatique après 5 échecs consécutifs de login
- **PasswordResetToken** : Jeton à usage unique et durée limitée (1h) permettant la réinitialisation du mot de passe
- **EmailVerificationToken** : Jeton à usage unique et durée limitée (24h) permettant la vérification de l'adresse email
- **Scope** : Permission déclarée par une ClientApplication que l'utilisateur autorise lors du consentement
- **Claim** : Assertion dans un AccessToken (userId, email, roles, permissions, clientId)
- **IdToken** : Jeton JWT OIDC contenant l'identité de l'utilisateur, retourné lors du flux Authorization Code

---

## Requirements

---

### Requirement 1: Flux de login complet avec refresh token

**User Story:** En tant qu'utilisateur, je veux me connecter avec mon email et mot de passe et recevoir un AccessToken et un RefreshToken, afin de pouvoir accéder aux ressources protégées et maintenir ma session sans me reconnecter constamment.

#### Acceptance Criteria

1. WHEN un User soumet des identifiants valides (email + mot de passe), THE Auth_Service SHALL retourner un AccessToken JWT (valide 15 minutes), un RefreshToken opaque (valide 30 jours), la date d'expiration du RefreshToken, et les informations de base du User (UserId, prénom, nom, email, rôles).
2. WHEN un User soumet un email inexistant ou un mot de passe incorrect, THE Auth_Service SHALL retourner HTTP 401 avec un message d'erreur générique ne révélant pas lequel des deux champs est incorrect.
3. WHEN un User soumet des identifiants valides, THE Session_Service SHALL créer une UserSession active en enregistrant l'adresse IP, le User-Agent, la date/heure de connexion, et en liant la session au RefreshToken émis.
4. IF le compte du User a `IsActive = false`, THEN THE Auth_Service SHALL retourner HTTP 401 avec le message "Compte désactivé. Contactez l'administrateur."
5. IF le compte du User a `IsLocked = true`, THEN THE Auth_Service SHALL retourner HTTP 401 avec le message "Compte verrouillé. Contactez l'administrateur." sans valider le mot de passe.
6. THE RefreshToken_Service SHALL générer les RefreshTokens avec une entropie cryptographique minimum de 256 bits via `RandomNumberGenerator`.
7. THE JWT_Service SHALL inclure dans l'AccessToken les claims : `sub` (UserId), `email`, `given_name`, `family_name`, `roles` (tableau), `permissions` (tableau), `jti` (UUID v4), `iat` (émission), `exp` (expiration).
8. THE RefreshToken_Service SHALL stocker un hash SHA-256 du RefreshToken en base de données, jamais la valeur en clair.

---

### Requirement 2: Rotation et révocation des refresh tokens

**User Story:** En tant que système, je veux que les refresh tokens soient rotés à chaque usage et révocables, afin de limiter l'impact d'un token compromis.

#### Acceptance Criteria

1. WHEN un client soumet un RefreshToken valide, THE RefreshToken_Service SHALL invalider l'ancien RefreshToken en enregistrant `RevokedAt = now` et `ReplacedByToken = hash du nouveau token`.
2. WHEN un client soumet un RefreshToken valide, THE RefreshToken_Service SHALL émettre un nouveau RefreshToken et retourner un nouveau AccessToken et le nouveau RefreshToken.
3. IF un client soumet un RefreshToken déjà révoqué ou expiré, THEN THE RefreshToken_Service SHALL retourner HTTP 401 avec le message "Refresh token invalide ou expiré."
4. IF un RefreshToken révoqué est soumis (détection de replay), THEN THE RefreshToken_Service SHALL révoquer atomiquement toute la chaîne de tokens liée au même User et invalider toutes les UserSessions actives de ce User.
5. WHEN la rotation d'un RefreshToken échoue (erreur base de données), THE RefreshToken_Service SHALL retourner HTTP 500 sans avoir émis de nouveau token.
6. THE RefreshToken_Service SHALL associer chaque RefreshToken à l'adresse IP et au User-Agent du client lors de son émission.
7. IF un RefreshToken est expiré depuis plus de 30 jours, THEN THE RefreshToken_Service SHALL le supprimer définitivement lors d'un nettoyage planifié quotidien.

---

### Requirement 3: Logout et invalidation de session

**User Story:** En tant qu'utilisateur, je veux pouvoir me déconnecter proprement, afin que ma session et mes tokens soient invalidés immédiatement.

#### Acceptance Criteria

1. WHEN un User authentifié envoie une requête de logout avec son RefreshToken, THE Auth_Service SHALL révoquer le RefreshToken et marquer la UserSession correspondante comme inactive (`IsActive = false`, `LogoutAt = now`).
2. WHEN un logout simple réussit, THE Auth_Service SHALL retourner HTTP 200.
3. WHEN un User demande un logout global ("logout all devices"), THE Auth_Service SHALL révoquer tous les RefreshTokens actifs du User et invalider toutes ses UserSessions actives, puis retourner HTTP 200.
4. IF le User n'a aucune session active lors d'un logout global, THEN THE Auth_Service SHALL retourner HTTP 200 (opération idempotente).
5. WHEN un logout simple ou global réussit, THE AuditLog_Service SHALL enregistrer un événement `Logout` avec UserId, IP et horodatage UTC.
6. IF un User envoie une requête de logout sans fournir de RefreshToken, THEN THE Auth_Service SHALL retourner HTTP 400.
7. IF un User envoie un RefreshToken invalide, expiré ou révoqué lors du logout, THEN THE Auth_Service SHALL retourner HTTP 401.
8. IF un AccessToken dont le `jti` a été révoqué est présenté à un endpoint protégé avant son expiration naturelle, THEN THE JWT_Service SHALL retourner HTTP 401.

---

### Requirement 4: Validation JWT et discovery OIDC

**User Story:** En tant que développeur d'application cliente, je veux pouvoir valider les tokens JWT émis par le SSO_Server et découvrir les endpoints OIDC automatiquement, afin d'intégrer le SSO sans gérer les clés de signature manuellement.

#### Acceptance Criteria

1. THE SSO_Server SHALL exposer un endpoint `POST /api/auth/validate-token` acceptant un AccessToken dans le corps de la requête (champ `token`) et retournant HTTP 200 avec les claims décodés (UserId, email, rôles, permissions) si le token est valide.
2. IF un token absent ou vide est soumis à `/api/auth/validate-token`, THEN THE JWT_Service SHALL retourner HTTP 400.
3. IF un token expiré, malformé ou dont la signature est invalide est soumis, THEN THE JWT_Service SHALL retourner HTTP 401 avec un champ `reason` indiquant le motif d'échec (`expired`, `invalid_signature`, `malformed`).
4. IF un token dont le `jti` est révoqué est soumis, THEN THE JWT_Service SHALL retourner HTTP 401 avec `reason: revoked`.
5. THE SSO_Server SHALL exposer un endpoint `GET /.well-known/openid-configuration` retournant les métadonnées OIDC (issuer, jwks_uri, authorization_endpoint, token_endpoint) sans authentification requise.
6. THE SSO_Server SHALL exposer un endpoint `GET /.well-known/jwks.json` retournant la clé publique de signature JWT au format JWKS sans authentification requise.

---

### Requirement 5: Gestion des applications clientes (ClientApplications)

**User Story:** En tant qu'administrateur SSO, je veux enregistrer et gérer les applications clientes avec leurs paramètres OAuth2/OIDC, afin de contrôler quelles applications peuvent s'authentifier via le SSO.

#### Acceptance Criteria

1. WHEN un flux d'authentification est initié avec un `ClientId`, THE ClientApp_Service SHALL vérifier que ce `ClientId` correspond à une ClientApplication avec `IsActive = true`; IF non, THEN retourner HTTP 401.
2. WHEN une ClientApplication avec `RequirePkce = true` initie un flux d'autorisation sans `code_challenge`, THE ClientApp_Service SHALL retourner HTTP 400 indiquant que PKCE est requis.
3. WHEN un `RedirectUri` est soumis lors d'un flux d'autorisation, THE ClientApp_Service SHALL vérifier qu'il correspond exactement (caractère par caractère, sans wildcard) à l'un des URIs enregistrés; IF non, THEN retourner HTTP 400.
4. IF le `ClientSecret` est stocké, THEN THE ClientApp_Service SHALL stocker sa valeur hachée (BCrypt ou SHA-256), sauf lors de la création initiale et de la rotation où la valeur en clair est retournée une seule fois.
5. WHEN une ClientApplication est créée, THE ClientApp_Service SHALL générer automatiquement un `ClientId` (GUID) et un `ClientSecret` cryptographiquement aléatoire (≥256 bits), et retourner le `ClientSecret` en clair une seule et unique fois dans la réponse de création.
6. WHEN un administrateur demande la rotation du secret d'une ClientApplication existante, THE ClientApp_Service SHALL invalider l'ancien secret, générer un nouveau `ClientSecret` (≥256 bits), le stocker hashé, et retourner la nouvelle valeur en clair une seule fois.
7. IF la ClientApplication cible de la rotation n'existe pas ou est inactive, THEN THE ClientApp_Service SHALL retourner HTTP 404.
8. IF la ClientApplication a `RequireConsent = false`, THEN THE Auth_Service SHALL omettre l'étape de consentement et accorder les scopes automatiquement.

---

### Requirement 6: Flux SSO multi-applications (Authorization Code Flow + PKCE)

**User Story:** En tant qu'utilisateur, je veux me connecter une seule fois via le SSO et accéder aux trois applications clientes sans avoir à me reconnecter, afin d'avoir une expérience d'authentification fluide.

#### Acceptance Criteria

1. THE SSO_Server SHALL implémenter le flux Authorization Code avec PKCE (RFC 7636) via les endpoints `GET /connect/authorize`, `POST /connect/token`, et `POST /connect/logout`.
2. WHEN un User non authentifié accède à `GET /connect/authorize` avec des paramètres valides (`ClientId`, `RedirectUri`, `ResponseType=code`, `Scope`, `State`, `CodeChallenge`), THE Auth_Service SHALL afficher la page de login SSO.
3. WHEN un User dispose d'une session SSO active valide et accède à `GET /connect/authorize`, THE Auth_Service SHALL émettre directement un code d'autorisation sans redemander le login.
4. WHEN un code d'autorisation valide est échangé via `POST /connect/token` avec `ClientId`, `CodeVerifier` et `RedirectUri`, THE Auth_Service SHALL retourner un AccessToken (≤1h), un RefreshToken (≤24h) et un IdToken OIDC (≤1h).
5. IF le `CodeVerifier` ne correspond pas au `CodeChallenge` initial lors de l'échange, THEN THE Auth_Service SHALL retourner HTTP 400 avec `error: invalid_grant`.
6. THE SSO_Server SHALL émettre un IdToken JWT contenant les claims OIDC : `sub`, `iss`, `aud`, `iat`, `exp`, `email`, `given_name`, `family_name`.
7. IF un code d'autorisation a déjà été utilisé, THEN THE Auth_Service SHALL retourner HTTP 400 avec `error: invalid_grant` (prévention de réutilisation).
8. IF un code d'autorisation n'est pas échangé dans les 10 minutes suivant son émission, THEN THE Auth_Service SHALL retourner HTTP 400 avec `error: expired_token`.
9. THE SSO_Server SHALL maintenir une session SSO côté serveur via un cookie sécurisé (HttpOnly, Secure, SameSite=Lax) d'une durée maximale de 8 heures.
10. WHEN un User se déconnecte via `POST /connect/logout` avec un `id_token_hint` valide, THE Auth_Service SHALL invalider la session SSO, révoquer tous les tokens associés, et rediriger vers le `PostLogoutRedirectUri` enregistré.
11. IF aucun `id_token_hint` n'est fourni lors du logout SSO, THEN THE Auth_Service SHALL invalider la session SSO courante basée sur le cookie de session et retourner HTTP 200.

---

### Requirement 7: Journalisation d'audit (AuditLogs)

**User Story:** En tant qu'administrateur de sécurité, je veux que toutes les opérations sensibles soient journalisées automatiquement, afin de pouvoir auditer les accès et détecter des comportements anormaux.

#### Acceptance Criteria

1. WHEN un événement de type `Login`, `LoginFailed`, `Logout`, `RefreshToken`, `RevokeToken`, `AccountUnlocked`, `Create`, `Update`, ou `Delete` se produit sur les entités User, Role, Permission ou ClientApplication, THE AuditLog_Service SHALL persister un AuditLog.
2. WHEN un AuditLog est créé, THE AuditLog_Service SHALL stocker : `UserId` (null si non authentifié), `Action`, `EntityName`, `EntityId` (null pour les actions non-CRUD), `OldValues` (null pour Create), `NewValues` (null pour Delete et actions non-CRUD), `IpAddress`, `UserAgent`, et `CreatedAt` (UTC).
3. THE AuditLog_Service SHALL enregistrer les événements de sécurité indépendamment de la logique métier des services existants, sans modifier leur comportement observable.
4. WHEN une entité est supprimée, THE AuditLog_Service SHALL sérialiser l'état complet de l'entité avant sa suppression dans le champ `OldValues` (JSON).
5. IF un utilisateur possédant le rôle `SuperAdmin` interroge `GET /api/audit-logs`, THEN THE SSO_Server SHALL retourner les logs filtrables par `UserId`, `Action`, `EntityName`, plage de dates, avec pagination (page 1–100, taille par défaut 50).
6. IF l'opération principale échoue partiellement, THEN THE AuditLog_Service SHALL persister le log de l'événement (fire-and-forget) afin de garantir la traçabilité même en cas d'erreur.
7. IF la persistance du log échoue, THEN THE AuditLog_Service SHALL enregistrer l'erreur dans le logger applicatif sans propager l'exception à l'opération principale.

---

### Requirement 8: Sécurité avancée — Gestion des mots de passe

**User Story:** En tant qu'utilisateur, je veux pouvoir réinitialiser mon mot de passe oublié et changer mon mot de passe, afin de maintenir la sécurité de mon compte.

#### Acceptance Criteria

1. WHEN un User demande une réinitialisation via `POST /api/auth/forgot-password`, THE Password_Service SHALL retourner HTTP 200 avec un message générique (ne confirmant pas l'existence de l'email), quelle que soit la validité de l'email.
2. IF l'email fourni à `forgot-password` a un format invalide, THEN THE Password_Service SHALL retourner HTTP 400.
3. IF l'email fourni est valide et correspond à un compte existant, THEN THE Password_Service SHALL générer un PasswordResetToken (usage unique, valide 3600 secondes), le stocker hashé, et l'envoyer via le service de notification.
4. WHEN un User soumet un PasswordResetToken valide (correspondance hash en base + non expiré + non utilisé) et un nouveau mot de passe, THE Password_Service SHALL hacher le nouveau mot de passe avec BCrypt, mettre à jour le compte, invalider tous les RefreshTokens actifs, invalider toutes les UserSessions actives, et invalider le token utilisé.
5. IF un PasswordResetToken expiré ou déjà utilisé est soumis, THEN THE Password_Service SHALL retourner HTTP 400.
6. WHEN un User authentifié soumet `POST /api/auth/change-password` avec l'ancien et le nouveau mot de passe, THE Password_Service SHALL vérifier l'ancien mot de passe, hacher le nouveau, mettre à jour le compte, et révoquer toutes les sessions sauf la UserSession identifiée par le RefreshToken de la requête courante.
7. IF l'ancien mot de passe soumis lors d'un `change-password` est incorrect, THEN THE Password_Service SHALL retourner HTTP 400.
8. IF un nouveau mot de passe ne respecte pas la politique (8–128 caractères, ≥1 majuscule, ≥1 chiffre, ≥1 caractère spécial ASCII codes 33–47/58–64/91–96/123–126), THEN THE Password_Service SHALL retourner HTTP 400.
9. IF le nouveau mot de passe est identique à l'actuel, THEN THE Password_Service SHALL retourner HTTP 400.

---

### Requirement 9: Sécurité avancée — Blocage de compte

**User Story:** En tant qu'administrateur de sécurité, je veux que les comptes soient automatiquement bloqués après plusieurs tentatives de connexion échouées, afin de protéger le système contre les attaques par force brute.

#### Acceptance Criteria

1. WHEN un login échoue (mot de passe incorrect), THE Auth_Service SHALL incrémenter `FailedLoginAttempts` du User et mettre à jour `LastFailedLoginAt = now`.
2. WHEN `FailedLoginAttempts` atteint 5, THE Auth_Service SHALL verrouiller le compte (`IsLocked = true`, `LockedAt = now`) et retourner HTTP 401 avec un message indiquant le verrouillage.
3. IF un User tente de se connecter alors que `IsLocked = true`, THEN THE Auth_Service SHALL retourner HTTP 401 sans valider le mot de passe.
4. WHEN un login réussit, THE Auth_Service SHALL remettre à zéro `FailedLoginAttempts = 0` et `LastFailedLoginAt = null`.
5. IF un compte avait `IsLocked = true` et a été déverrouillé manuellement, THEN lors du prochain login réussi, THE Auth_Service SHALL également remettre `IsLocked = false`.
6. WHEN un administrateur déverrouille un compte via `POST /api/users/{id}/unlock`, THE Auth_Service SHALL remettre `IsLocked = false`, `FailedLoginAttempts = 0`, et THE AuditLog_Service SHALL enregistrer un événement `AccountUnlocked` avec l'identité de l'administrateur.
7. THE User entity SHALL stocker les champs : `FailedLoginAttempts` (int, défaut 0), `LastFailedLoginAt` (DateTime?, nullable), `IsLocked` (bool, défaut false), `LockedAt` (DateTime?, nullable).

---

### Requirement 10: Vérification d'email

**User Story:** En tant que nouvel utilisateur, je veux vérifier mon adresse email, afin de confirmer que je suis propriétaire du compte.

#### Acceptance Criteria

1. WHEN un User est créé, THE Auth_Service SHALL générer un EmailVerificationToken (usage unique, valide 86400 secondes), le stocker hashé en base de données, et l'envoyer via le service de notification.
2. WHEN un User soumet un EmailVerificationToken valide (correspondance hash + non expiré + compte non déjà vérifié) via `GET /api/auth/verify-email?token={token}`, THE Auth_Service SHALL marquer `IsEmailVerified = true`, supprimer le token stocké, et retourner HTTP 200.
3. IF un EmailVerificationToken expiré, invalide ou déjà utilisé est soumis, THEN THE Auth_Service SHALL retourner HTTP 400.
4. THE User entity SHALL stocker : `IsEmailVerified` (bool, défaut false), `EmailVerificationToken` (string hashé, nullable), `EmailVerificationTokenExpiresAt` (DateTime?, nullable).
5. WHILE la configuration `EmailVerification:Required = true` est active, IF un User tente de se connecter avec `IsEmailVerified = false`, THEN THE Auth_Service SHALL retourner HTTP 401.
6. WHEN un User demande un renvoi du token via `POST /api/auth/resend-verification`, THE Auth_Service SHALL invalider le token précédent, générer un nouveau token, le stocker hashé, et l'envoyer. IF le User a effectué plus de 3 demandes de renvoi dans la dernière heure, THEN THE Auth_Service SHALL retourner HTTP 429.

---

### Requirement 11: Documentation Swagger et README

**User Story:** En tant que développeur intégrant une application cliente, je veux une documentation API complète et un README clair, afin de comprendre comment intégrer le SSO sans avoir à lire le code source.

#### Acceptance Criteria

1. THE SSO_Server SHALL exposer une interface Swagger UI accessible à `GET /swagger` sans authentification requise, documentant tous les endpoints avec paramètres, corps de requête, codes HTTP de réponse et schémas.
2. THE SSO_Server SHALL configurer Swagger avec support de l'authentification Bearer JWT, permettant de tester les endpoints protégés depuis l'interface Swagger UI.
3. WHEN un endpoint retourne une réponse 4xx ou 5xx, THE SSO_Server SHALL retourner un objet JSON `ProblemDetails` (RFC 7807) contenant `type`, `title`, `status`, `detail`, et `traceId`.
4. THE SSO_Server SHALL inclure des commentaires XML sur tous les contrôleurs et DTOs publics.
5. WHEN le projet est livré, THE SSO_Server SHALL inclure un fichier `README.md` documentant : l'architecture, les prérequis, la configuration, les endpoints principaux, le guide d'intégration des applications clientes, et un exemple de flux SSO complet.

---

### Requirement 12: Intégration des trois applications clientes existantes

**User Story:** En tant que développeur des applications clientes, je veux adapter l'authentification de mes applications existantes pour utiliser ONEE.SSO comme Identity Provider, sans reconstruire les interfaces utilisateur.

#### Acceptance Criteria

1. THE SSO_Server SHALL fournir pour chaque application cliente enregistrée un `ClientId`, un `ClientSecret`, un ou plusieurs `RedirectUri`, et les scopes autorisés, configurables via l'API `ClientApplications` ou le seed de base de données.
2. WHEN une application cliente présente un AccessToken à `POST /api/auth/validate-token`, THE SSO_Server SHALL retourner HTTP 200 avec les claims (UserId, email, given_name, family_name, roles, permissions) si le token est valide.
3. IF une application cliente présente un AccessToken valide à `POST /api/auth/validate-token` mais avec un `aud` claim ne correspondant pas à son `ClientId`, THEN THE JWT_Service SHALL retourner HTTP 401.
4. IF l'AccessToken présenté à `POST /api/auth/validate-token` est invalide, expiré ou révoqué, THEN THE SSO_Server SHALL retourner HTTP 401.
5. THE SSO_Server SHALL exposer un endpoint `GET /api/auth/userinfo` retournant les claims OIDC (sub, email, given_name, family_name, roles, permissions) pour le porteur d'un AccessToken valide.
6. IF l'AccessToken présenté à `GET /api/auth/userinfo` est absent, invalide ou expiré, THEN THE SSO_Server SHALL retourner HTTP 401.
7. THE SSO_Server SHALL produire un guide d'intégration documentant les modifications minimales requises pour connecter une application cliente existante au flux SSO.
