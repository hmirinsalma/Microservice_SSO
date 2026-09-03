# 🤝 Contributing to ONEE SSO

Merci de votre intérêt pour contribuer au projet ONEE SSO !

## 🚀 Setup du projet

1. **Cloner le repository**
   ```bash
   git clone https://github.com/VOTRE_USERNAME/ONEE.SSO.git
   cd ONEE.SSO
   ```

2. **Installer les prérequis**
   - .NET SDK 9.0+
   - Node.js 18+
   - SQL Server (LocalDB ou Express)
   - PowerShell 7+

3. **Configuration**
   - Copier `src/ONEE.SSO.API/appsettings.example.json` vers `appsettings.json`
   - Ajuster la chaîne de connexion SQL Server
   - Configurer les secrets JWT et OIDC

4. **Lancer le projet**
   ```powershell
   .\SETUP_COMPLET.ps1
   ```

## 📝 Guidelines

### Code Style

- **Backend (.NET)** : Suivre les conventions C# standards
- **Frontend (React)** : Utiliser ESLint + Prettier
- **Indentation** : 2 espaces pour JS/JSX, 4 espaces pour C#
- **Nommage** : PascalCase pour C#, camelCase pour JavaScript

### Commits

Utiliser des messages de commit clairs et descriptifs :

```
feat: ajouter authentification multi-facteurs
fix: corriger bug de redirection après login
docs: mettre à jour README avec nouvelles instructions
refactor: simplifier logique de validation JWT
```

Préfixes recommandés :
- `feat`: Nouvelle fonctionnalité
- `fix`: Correction de bug
- `docs`: Documentation
- `refactor`: Refactoring
- `test`: Tests
- `chore`: Tâches diverses

### Pull Requests

1. **Fork** le repository
2. Créer une **branche** pour votre feature (`git checkout -b feature/ma-feature`)
3. **Committer** vos changements (`git commit -m 'feat: ajouter ma feature'`)
4. **Pusher** vers votre fork (`git push origin feature/ma-feature`)
5. Ouvrir une **Pull Request**

**Structure de la PR** :
```markdown
## Description
[Description claire de ce qui a été modifié]

## Type de changement
- [ ] Bug fix
- [ ] Nouvelle fonctionnalité
- [ ] Breaking change
- [ ] Documentation

## Tests effectués
- [x] Test 1
- [x] Test 2

## Checklist
- [ ] Mon code suit les conventions du projet
- [ ] J'ai ajouté des tests si nécessaire
- [ ] J'ai mis à jour la documentation
- [ ] Aucun warning/erreur dans les logs
```

## 🧪 Tests

Avant de soumettre une PR :

```bash
# Backend
dotnet test

# Frontend RH
cd clients/gestion-personnel/frontend
npm run test

# Frontend TIMS
cd clients/tims/frontend
npm run test

# Frontend EAMS
cd clients/eams/frontend
npm run test
```

## 🐛 Signaler un bug

Utiliser les **GitHub Issues** avec le template suivant :

```markdown
### Description du bug
[Description claire du problème]

### Étapes pour reproduire
1. Aller à '...'
2. Cliquer sur '...'
3. Voir l'erreur

### Comportement attendu
[Ce qui devrait se passer]

### Comportement actuel
[Ce qui se passe réellement]

### Environnement
- OS: [ex: Windows 11]
- Node: [ex: v18.17.0]
- .NET: [ex: 9.0]
- Navigateur: [ex: Chrome 120]

### Screenshots
[Si applicable]
```

## 💡 Proposer une fonctionnalité

Ouvrir une **GitHub Issue** avec le tag `enhancement` :

```markdown
### Description de la fonctionnalité
[Description claire de ce que vous proposez]

### Pourquoi cette fonctionnalité ?
[Expliquer le besoin / cas d'usage]

### Solution proposée
[Comment l'implémenter]

### Alternatives considérées
[Autres approches possibles]
```

## 📚 Structure du projet

```
ONEE.SSO/
├── src/                    # Microservice SSO (backend)
│   ├── ONEE.SSO.API/      # API et pages Razor
│   ├── ONEE.SSO.Application/
│   ├── ONEE.SSO.Domain/
│   ├── ONEE.SSO.Infrastructure/
│   └── ONEE.SSO.Shared/
│
├── clients/                # Applications clientes
│   ├── gestion-personnel/  # RH
│   ├── tims/               # Interventions techniques
│   └── eams/               # Gestion équipements
│
└── scripts/                # Scripts utiles
```

## ❓ Questions ?

- Ouvrir une **GitHub Issue**
- Consulter la [Documentation](./README.md)
- Consulter le [Guide de Sécurité](./SECURITY.md)

## 📜 License

En contribuant, vous acceptez que vos contributions soient sous la même license que le projet.

---

**Merci pour votre contribution ! 🎉**
