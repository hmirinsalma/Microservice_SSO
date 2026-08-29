# 📋 QUESTIONS SUR LES 3 APPLICATIONS CLIENTES EXISTANTES

Pour intégrer correctement le SSO avec vos 3 applications **SANS les recréer**, j'ai besoin des informations suivantes pour CHAQUE application :

---

## 🔷 APPLICATION 1 : GESTION PERSONNEL (RH)

### Technologie & Infrastructure
1. **Framework/Technologie frontend** : React ? Angular ? Vue ? Blazor ? ASP.NET MVC ? Autre ?
2. **Version** : (ex: React 18, Angular 17, Vue 3, etc.)
3. **Port de l'application frontend** : (ex: http://localhost:3000, http://localhost:5173, etc.)
4. **Chemin du dossier du projet** : (ex: C:\projets\gestion-rh-frontend)

### Backend
5. **Backend séparé** : Oui / Non
6. **Si oui, technologie backend** : ASP.NET Core ? Node.js ? Autre ?
7. **Si oui, port du backend** : (ex: http://localhost:5000)
8. **Si oui, chemin du dossier backend** : (ex: C:\projets\gestion-rh-backend)

### Système d'Authentification Actuel
9. **Comment l'utilisateur se connecte actuellement** :
   - Formulaire login/password dans l'application ?
   - Redirection vers un service externe ?
   - Autre ?

10. **Si formulaire local, que se passe-t-il après submit** :
    - Appel API : quelle URL ? (ex: POST /api/auth/login)
    - Réponse attendue : token ? session ? cookie ?
    - Où est stocké le token/session : localStorage ? sessionStorage ? cookie ?

11. **Gestion actuelle des utilisateurs** :
    - Table Users en base de données locale ?
    - Appel à un service externe ?
    - Autre ?

12. **Base de données** :
    - SQL Server local ? Distant ?
    - Autre base ?
    - Connection string ?

### Routing & Navigation
13. **Système de routing** : React Router ? Angular Router ? Vue Router ? Autre ?
14. **Page de login actuelle** : Quelle route ? (ex: /login, /auth/login, /)
15. **Page après login réussi** : Quelle route ? (ex: /dashboard, /home, /)

### Configuration Souhaitée pour SSO
16. **URL callback souhaitée après authentification SSO** : (ex: /callback, /auth/callback, /sso/callback)
17. **URL de déconnexion** : (ex: /logout, /auth/logout)

### Informations Utilisateur
18. **Données utilisateur nécessaires dans l'application** :
    - UserId ?
    - Email ?
    - Nom/Prénom ?
    - Rôles ?
    - Permissions ?
    - Autres champs spécifiques ?

19. **Où sont utilisées ces données** :
    - Affichage du nom dans le header ?
    - Restrictions d'accès à certaines pages ?
    - Appels API avec token Bearer ?
    - Autres usages ?

---

## 🔷 APPLICATION 2 : TIMS

### Technologie & Infrastructure
1. **Framework/Technologie frontend** :
2. **Version** :
3. **Port de l'application frontend** :
4. **Chemin du dossier du projet** :

### Backend
5. **Backend séparé** :
6. **Si oui, technologie backend** :
7. **Si oui, port du backend** :
8. **Si oui, chemin du dossier backend** :

### Système d'Authentification Actuel
9. **Comment l'utilisateur se connecte actuellement** :
10. **Si formulaire local, que se passe-t-il après submit** :
11. **Gestion actuelle des utilisateurs** :
12. **Base de données** :

### Routing & Navigation
13. **Système de routing** :
14. **Page de login actuelle** :
15. **Page après login réussi** :

### Configuration Souhaitée pour SSO
16. **URL callback souhaitée après authentification SSO** :
17. **URL de déconnexion** :

### Informations Utilisateur
18. **Données utilisateur nécessaires dans l'application** :
19. **Où sont utilisées ces données** :

### Scopes Spécifiques TIMS
20. **Scopes custom définis** : tims_user_id, tims_service_id, tims_team_id
21. **À quoi servent ces scopes** :
    - tims_user_id : ?
    - tims_service_id : ?
    - tims_team_id : ?

---

## 🔷 APPLICATION 3 : EAMS

### Technologie & Infrastructure
1. **Framework/Technologie frontend** :
2. **Version** :
3. **Port de l'application frontend** :
4. **Chemin du dossier du projet** :

### Backend
5. **Backend séparé** :
6. **Si oui, technologie backend** :
7. **Si oui, port du backend** :
8. **Si oui, chemin du dossier backend** :

### Système d'Authentification Actuel
9. **Comment l'utilisateur se connecte actuellement** :
10. **Si formulaire local, que se passe-t-il après submit** :
11. **Gestion actuelle des utilisateurs** :
12. **Base de données** :

### Routing & Navigation
13. **Système de routing** :
14. **Page de login actuelle** :
15. **Page après login réussi** :

### Configuration Souhaitée pour SSO
16. **URL callback souhaitée après authentification SSO** :
17. **URL de déconnexion** :

### Informations Utilisateur
18. **Données utilisateur nécessaires dans l'application** :
19. **Où sont utilisées ces données** :

### Scopes Spécifiques EAMS
20. **Scopes custom définis** : eams_user_id, serviceId
21. **À quoi servent ces scopes** :
    - eams_user_id : ?
    - serviceId : ?

---

## 📝 FORMAT DE RÉPONSE SUGGÉRÉ

Vous pouvez répondre dans ce format pour plus de clarté :

```
═══════════════════════════════════════════════════════════════
APPLICATION RH (GESTION PERSONNEL)
═══════════════════════════════════════════════════════════════

TECHNOLOGIE:
- Framework: React 18
- Port frontend: http://localhost:5173
- Dossier: C:\projets\gestion-rh-frontend

BACKEND:
- Backend séparé: Oui
- Technologie: ASP.NET Core 8
- Port: http://localhost:5000
- Dossier: C:\projets\gestion-rh-backend

AUTHENTIFICATION ACTUELLE:
- Login: Formulaire local → POST /api/auth/login
- Stockage: localStorage("token")
- Users: Table Users en SQL Server local

ROUTING:
- Router: React Router v6
- Page login: /login
- Page après login: /dashboard

SSO CONFIG:
- Callback URL: /callback
- Logout URL: /logout

DONNÉES UTILISATEUR:
- Nécessaires: UserId, Email, Nom, Prénom, Rôles, Permissions
- Usage: Header (affichage nom), Routes protégées, API calls Bearer

═══════════════════════════════════════════════════════════════
APPLICATION TIMS
═══════════════════════════════════════════════════════════════

TECHNOLOGIE:
- Framework: Angular 17
- Port frontend: http://localhost:4200
- Dossier: C:\projets\tims-frontend

... (même structure)

SCOPES CUSTOM:
- tims_user_id: Identifiant unique utilisateur dans TIMS
- tims_service_id: Service/département de l'utilisateur
- tims_team_id: Équipe de l'utilisateur dans TIMS

═══════════════════════════════════════════════════════════════
APPLICATION EAMS
═══════════════════════════════════════════════════════════════

TECHNOLOGIE:
- Framework: Vue 3
- Port frontend: http://localhost:4202
- Dossier: C:\projets\eams-frontend

... (même structure)

SCOPES CUSTOM:
- eams_user_id: Identifiant utilisateur EAMS
- serviceId: Service de rattachement
```

---

## ⚡ INFORMATIONS COMPLÉMENTAIRES IMPORTANTES

### Gestion des Tokens
- Comment vos applications gèrent-elles actuellement l'expiration des tokens ?
- Y a-t-il un mécanisme de refresh automatique ?
- Que se passe-t-il quand le token expire ?

### Sécurité
- Y a-t-il des intercepteurs HTTP pour ajouter automatiquement le token aux requêtes ?
- Y a-t-il des guards/middlewares pour protéger certaines routes ?

### Expérience Utilisateur Souhaitée
- Voulez-vous que l'utilisateur soit redirigé automatiquement vers le SSO si non connecté ?
- Voulez-vous garder un bouton "Se connecter" qui redirige vers le SSO ?
- Que doit-il se passer au logout ? Redirection vers le SSO ? Vers la page login de l'app ?

### Tests
- Avez-vous des utilisateurs de test dans vos applications actuelles ?
- Pouvez-vous me fournir 1-2 comptes test (username/password) pour chaque app ?

---

## 🎯 OBJECTIF

Avec ces informations, je pourrai vous fournir :

1. **Pour chaque application** : Un prompt avec EXACTEMENT le code/configuration à ajouter
2. **Modifications minimales** : Seulement ce qui est nécessaire pour l'intégration SSO
3. **Pas de refonte** : Conservation de votre code existant
4. **Tests guidés** : Étape par étape pour valider l'intégration
5. **Scénario SSO complet** : Login une fois → Accès aux 3 apps

---

## 📤 PROCHAINE ÉTAPE

Une fois ces informations fournies, je vous donnerai :

✅ **PROMPT 1** : Code exact pour intégrer l'Application RH au SSO
✅ **PROMPT 2** : Code exact pour intégrer TIMS au SSO  
✅ **PROMPT 3** : Code exact pour intégrer EAMS au SSO
✅ **PROMPT 4** : Tests étape par étape pour valider le SSO complet

Chaque prompt sera autonome et copier-coller prêt ! 🚀
