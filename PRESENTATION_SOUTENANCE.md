# GUIDE DE PRÉSENTATION - SOUTENANCE SSO ONEE

## 📋 PLAN DE PRÉSENTATION (15 minutes)

### 1. INTRODUCTION (2 minutes)

#### Contexte
"Bonjour, je vais vous présenter mon projet de fin d'études sur la mise en place d'un système d'authentification unique (SSO) pour l'ONEE.

L'Office National de l'Électricité et de l'Eau potable utilise actuellement plusieurs applications métier indépendantes, chacune avec son propre système d'authentification. Cette situation pose des problèmes:
- Gestion complexe des comptes utilisateurs
- Risques de sécurité accrus
- Expérience utilisateur dégradée"

#### Objectifs
"Mon projet consiste à centraliser l'authentification via un serveur SSO basé sur le protocole OIDC/OAuth2, permettant aux employés de se connecter une seule fois pour accéder à toutes les applications."

---

### 2. ARCHITECTURE TECHNIQUE (3 minutes)

#### Technologies Utilisées
"Le projet utilise des technologies modernes et robustes:
- **Backend**: ASP.NET Core 9 avec C# 13
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, API)
- **Base de données**: PostgreSQL avec Entity Framework Core
- **Authentification**: OIDC/OAuth2 avec JWT
- **Frontend**: Razor Pages pour l'admin, React pour les clients
- **Patterns**: Repository Pattern, Dependency Injection"

#### Architecture Clean
[Montrer le diagramme]
```
┌─────────────────────────────────────────────────┐
│              ONEE.SSO.API                       │
│  (Controllers, Pages, Middleware, Program.cs)   │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│         ONEE.SSO.Application                    │
│  (Services, DTOs, Interfaces, Business Logic)   │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│       ONEE.SSO.Infrastructure                   │
│  (Repositories, DbContext, JwtService, Security)│
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│           ONEE.SSO.Domain                       │
│        (Entities, Aggregates, Rules)            │
└─────────────────────────────────────────────────┘
```

#### Flow d'Authentification OIDC
[Montrer le schéma du flow]
```
1. User → Client App: "Se connecter avec SSO"
2. Client App → SSO: Redirect /connect/authorize
3. SSO: Afficher login
4. User → SSO: Email/Password
5. SSO: Afficher consentement
6. User → SSO: "Autoriser"
7. SSO → Client App: Redirect avec code
8. Client App → SSO: POST /connect/token (échange code)
9. SSO → Client App: access_token + id_token (JWT)
10. Client App → API Backend: Authorization: Bearer <token>
11. API Backend: Valider JWT, autoriser accès
```

---

### 3. DÉMONSTRATION (7 minutes)

#### Démo 1: Interface Admin (3 minutes)

**Dashboard**
"Voici l'interface d'administration du SSO. Le dashboard présente:
- Vue d'ensemble avec statistiques en temps réel
- Nombre d'utilisateurs actifs
- Sessions en cours
- Applications clientes configurées"

[Naviguer vers http://localhost:5205/Dashboard]

**Gestion des Utilisateurs**
"La gestion des utilisateurs permet de:
- Voir tous les utilisateurs avec recherche et filtres
- Consulter leurs rôles et permissions
- Activer/désactiver des comptes
- Supprimer des utilisateurs"

[Cliquer sur "Utilisateurs" dans le menu]
[Faire une recherche: "admin"]
[Filtrer par rôle: "Admin"]

**Gestion des Rôles et Permissions**
"Le système de rôles permet une gestion fine des droits:
- Créer des rôles personnalisés
- Assigner des permissions spécifiques
- Gérer les accès par application"

[Cliquer sur "Rôles"]
[Cliquer "Créer un rôle"]
[Montrer le formulaire]
[Cliquer "Permissions" sur un rôle]
[Montrer la gestion des permissions]

**Applications Clientes**
"Trois applications sont actuellement intégrées:
- Gestion Personnel (RH)
- TIMS (Technical Information Management)
- EAMS (Equipment Asset Management)"

[Cliquer sur "Applications"]
[Montrer les 3 cartes]

#### Démo 2: Flow SSO Complet (4 minutes)

**Étape 1: Accès à l'application**
"Un employé souhaite accéder à l'application Gestion Personnel."

[Ouvrir http://localhost:5173]
[Cliquer "Se connecter avec SSO"]

**Étape 2: Authentification**
"L'utilisateur est redirigé vers le serveur SSO pour s'authentifier."

[Page de login SSO s'affiche]
[Entrer: admin@onee.ma / Admin@123]
[Cliquer "Se connecter"]

**Étape 3: Consentement**
"Le système demande à l'utilisateur d'autoriser l'accès de l'application à ses informations."

[Page de consentement s'affiche]
[Montrer les scopes demandés]
[Cliquer "Autoriser"]

**Étape 4: Accès accordé**
"L'utilisateur accède directement à son dashboard RH sans nouvelle authentification."

[Dashboard RH s'affiche]
[Montrer la navigation]
[Montrer le menu utilisateur]

**Étape 5: Vérification Technique**
[Ouvrir DevTools F12 → Application → LocalStorage]
[Montrer le token JWT stocké]

[Ouvrir la console du backend RH]
[Montrer les logs de validation du token]

**Étape 6: Logout Centralisé**
"Le logout depuis l'application déconnecte l'utilisateur de toutes les applications."

[Cliquer "Se déconnecter"]
[Montrer retour au login]
[Vérifier que le token est supprimé du LocalStorage]

---

### 4. SÉCURITÉ (2 minutes)

#### Mécanismes de Sécurité
"Le système intègre plusieurs mécanismes de sécurité:

**1. JWT avec Signature**
- Tokens signés avec clé secrète
- Header contient le `kid` (Key ID) pour validation
- Claims inclus: user ID, email, rôles, permissions
- Expiration configurable (60 minutes par défaut)

**2. PKCE (Proof Key for Code Exchange)**
- Protection contre les attaques d'interception de code
- Code Challenge/Verifier obligatoire
- Validation stricte côté serveur

**3. Validation Stricte**
- Vérification client_id et client_secret
- Validation redirect_uri contre liste blanche
- Expiration des codes d'autorisation (5 minutes)
- Nettoyage automatique des codes expirés

**4. CORS Sécurisé**
- Liste blanche des origines autorisées
- Credentials requis
- Headers et méthodes contrôlés"

---

### 5. FONCTIONNALITÉS AVANCÉES (1 minute)

#### Fonctionnalités Implémentées
"Le système offre des fonctionnalités avancées:
- **Gestion des sessions**: Monitoring des sessions actives
- **Logs d'audit**: Traçabilité complète des actions
- **Paramètres système**: Configuration centralisée
- **Design responsive**: Compatible mobile et tablette
- **Seed automatique**: Données de démonstration"

#### Fonctionnalités Futures
"Des améliorations sont prévues:
- Refresh Tokens pour renouvellement automatique
- Two-Factor Authentication (2FA)
- Notifications email (SMTP)
- Rate Limiting contre les attaques
- Tests unitaires et d'intégration"

---

### 6. CONCLUSION ET OUVERTURE (1 minute)

#### Résultats
"Ce projet répond aux objectifs fixés:
- ✅ Centralisation de l'authentification
- ✅ Interface d'administration complète
- ✅ Intégration de 3 applications clientes
- ✅ Respect du standard OIDC
- ✅ Sécurité renforcée
- ✅ Architecture extensible"

#### Bénéfices pour l'ONEE
"Les bénéfices pour l'organisation:
- Réduction du temps de connexion pour les employés
- Gestion centralisée simplifiée
- Sécurité améliorée
- Traçabilité complète
- Facilité d'ajout de nouvelles applications"

#### Perspectives
"Ce projet constitue une base solide pour:
- Intégrer d'autres applications existantes
- Déployer en production rapidement
- Évoluer vers des besoins futurs (2FA, SSO social, etc.)"

---

## 🎯 QUESTIONS PROBABLES DU JURY

### Question 1: Pourquoi OIDC et pas SAML?
**Réponse**: "OIDC est plus moderne, plus simple à implémenter, et mieux adapté aux applications web et mobiles. Il utilise JSON et REST au lieu de XML, ce qui le rend plus performant et plus facile à intégrer."

### Question 2: Comment gérez-vous la sécurité des tokens?
**Réponse**: "Les tokens JWT sont signés avec une clé secrète, incluent un `kid` pour validation, ont une durée de vie limitée (60 min), et sont stockés côté client dans LocalStorage avec HttpOnly si possible. Nous avons aussi implémenté PKCE pour protéger le flow d'autorisation."

### Question 3: Que se passe-t-il si le serveur SSO tombe?
**Réponse**: "Les utilisateurs déjà connectés peuvent continuer à utiliser les applications tant que leur token est valide (60 min). Pour la production, on prévoirait une architecture haute disponibilité avec plusieurs instances SSO derrière un load balancer."

### Question 4: Comment gérez-vous les rôles différents selon les applications?
**Réponse**: "Les rôles et permissions sont stockés dans le token JWT. Chaque application cliente peut lire les claims du token et vérifier si l'utilisateur a les permissions nécessaires pour accéder à une ressource spécifique."

### Question 5: Avez-vous prévu des tests?
**Réponse**: "Le projet inclut des tests manuels documentés dans le guide de test. Pour la production, j'ai identifié les tests unitaires (services, repositories) et d'intégration (flow OIDC complet) à implémenter."

### Question 6: Pourquoi PostgreSQL et pas SQL Server?
**Réponse**: "PostgreSQL est open-source, performant, robuste, et largement utilisé en entreprise. Il offre toutes les fonctionnalités nécessaires (transactions ACID, indexes, etc.) et est compatible avec Entity Framework Core."

### Question 7: Comment gérez-vous la révocation des tokens?
**Réponse**: "Actuellement, les tokens JWT sont stateless et expirent après 60 minutes. Pour une gestion plus fine, on peut implémenter une blacklist de JTI (JWT ID) stockée en cache Redis, ou passer à des refresh tokens révocables."

### Question 8: L'interface admin est-elle sécurisée?
**Réponse**: "Oui, l'interface admin nécessite une authentification, et on peut restreindre l'accès au rôle Admin uniquement. Pour la production, on ajouterait une protection CSRF et un audit des actions administratives."

---

## 📊 SLIDES SUGGÉRÉS

### Slide 1: Page de Titre
```
MISE EN PLACE D'UN SYSTÈME SSO
POUR L'ONEE

[Votre Nom]
[Date]
[Logo ONEE]
```

### Slide 2: Contexte et Problématique
```
CONTEXTE
• ONEE: 3+ applications métier
• Authentification décentralisée
• Gestion complexe des comptes

PROBLÈMES
• Multiples logins/passwords
• Risques de sécurité
• Expérience utilisateur dégradée
```

### Slide 3: Objectifs
```
OBJECTIFS DU PROJET
✓ Centraliser l'authentification
✓ Implémenter le protocole OIDC
✓ Interface d'administration
✓ Intégrer 3 applications
✓ Sécuriser les échanges
```

### Slide 4: Architecture Technique
```
ARCHITECTURE CLEAN
[Diagramme des couches]

TECHNOLOGIES
• ASP.NET Core 9
• PostgreSQL + EF Core
• JWT + OIDC/OAuth2
• React + Razor Pages
```

### Slide 5: Flow OIDC
```
FLOW D'AUTHENTIFICATION
[Diagramme de séquence]

1. Demande d'autorisation
2. Authentification utilisateur
3. Consentement
4. Échange code → token
5. Accès aux ressources
```

### Slide 6: Sécurité
```
MÉCANISMES DE SÉCURITÉ
• JWT signé avec kid
• PKCE (Code Challenge)
• Validation stricte
• CORS sécurisé
• Expiration des tokens
• Audit des actions
```

### Slide 7: Interface Admin
```
INTERFACE D'ADMINISTRATION
[Screenshots]

• Dashboard avec statistiques
• Gestion utilisateurs/rôles
• Applications clientes
• Sessions actives
• Logs d'audit
```

### Slide 8: Applications Intégrées
```
APPLICATIONS CLIENTES
1. Gestion Personnel (RH)
2. TIMS (Tech Management)
3. EAMS (Asset Management)

[Logos/Screenshots]
```

### Slide 9: Résultats
```
RÉSULTATS
✓ SSO fonctionnel
✓ 3 applications intégrées
✓ Interface admin complète
✓ Architecture extensible
✓ Sécurité renforcée
```

### Slide 10: Perspectives
```
PERSPECTIVES D'ÉVOLUTION
• Refresh Tokens
• Two-Factor Authentication
• Intégration SSO social
• Déploiement production
• Haute disponibilité
```

### Slide 11: Conclusion
```
CONCLUSION
Un système SSO moderne et sécurisé
prêt pour la production

BÉNÉFICES ONEE
• Expérience utilisateur améliorée
• Gestion simplifiée
• Sécurité renforcée
• Évolutivité garantie
```

### Slide 12: Questions
```
MERCI DE VOTRE ATTENTION

QUESTIONS ?

[Vos coordonnées]
[Email]
```

---

## ✅ CHECKLIST AVANT PRÉSENTATION

### La Veille
- [ ] Tester le flow complet 3 fois
- [ ] Vérifier que tous les services démarrent sans erreur
- [ ] Préparer un backup du projet (ZIP)
- [ ] Revoir la documentation
- [ ] Répéter la présentation (chronométrer)
- [ ] Préparer les réponses aux questions probables
- [ ] Charger les batteries du laptop
- [ ] Préparer une clé USB de secours

### Le Jour J
- [ ] Arriver 15 minutes en avance
- [ ] Tester la connexion vidéo/projecteur
- [ ] Démarrer tous les services
- [ ] Ouvrir les onglets nécessaires
- [ ] Mettre le téléphone en silencieux
- [ ] Respirer profondément et sourire 😊

### Pendant la Présentation
- [ ] Parler clairement et lentement
- [ ] Regarder le jury
- [ ] Montrer l'enthousiasme
- [ ] Gérer le timing
- [ ] Faire les démos sans précipitation
- [ ] Écouter les questions attentivement
- [ ] Répondre avec confiance

---

## 🎯 CONSEILS PRATIQUES

### Pour la Démo
1. **Préparer des comptes de test** multiples au cas où
2. **Avoir un plan B** si un service ne démarre pas
3. **Prendre son temps** - ne pas précipiter les actions
4. **Expliquer ce qu'on fait** pendant la manipulation
5. **Zoom sur les éléments importants** (JWT, logs, etc.)

### Pour les Questions
1. **Écouter jusqu'au bout** avant de répondre
2. **Reformuler** si la question n'est pas claire
3. **Être honnête** si on ne sait pas
4. **Rester calme** même si la question est difficile
5. **Faire le lien** avec le projet si possible

### Timing
- Introduction: 2 min MAX
- Architecture: 3 min MAX
- Démo: 7 min (ne pas dépasser!)
- Sécurité: 2 min
- Conclusion: 1 min

**Total: 15 minutes** (laisser du temps pour les questions)

---

## 🏆 POINTS FORTS À METTRE EN AVANT

1. **Architecture Professionnelle** - Clean Architecture, patterns modernes
2. **Standard OIDC** - Protocole reconnu mondialement
3. **Sécurité** - JWT, PKCE, validation stricte
4. **Interface Moderne** - Design professionnel ONEE
5. **Extensibilité** - Facile d'ajouter des applications
6. **Documentation** - Complète et détaillée
7. **Démo Fonctionnelle** - Tout marche en live!

---

## 💪 MOTIVATION FINALE

**Vous avez créé un vrai projet professionnel!**

- Architecture solide ✅
- Code propre ✅
- Démo fonctionnelle ✅
- Documentation complète ✅

**Soyez confiant et fier de votre travail!**

Le jury va apprécier:
- La qualité technique
- L'interface moderne
- La démo fluide
- Votre maîtrise du sujet

**Vous êtes prêt! Bonne chance! 🚀**

---

**Document préparé le**: 24 Août 2026  
**Pour**: Soutenance SSO ONEE  
**Status**: ✅ Prêt à présenter
