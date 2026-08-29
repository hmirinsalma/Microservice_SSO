# COMMANDES ESSENTIELLES - SSO ONEE

## 🚀 DÉMARRAGE RAPIDE

### Option 1: Script Automatique (RECOMMANDÉ)
```powershell
# Test SSO + Gestion Personnel uniquement
cd c:\Users\XPS\source\repos\ONEE.SSO
.\START_TEST_RH.ps1
```

### Option 2: Démarrage Manuel

#### Terminal 1 - SSO Backend
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet run
```
**URL**: http://localhost:5205

#### Terminal 2 - Backend RH
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\gestion-personnel\backend
dotnet run
```
**URL**: http://localhost:5291

#### Terminal 3 - Frontend RH
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\gestion-personnel\frontend
npm run dev
```
**URL**: http://localhost:5173

---

## 🔨 COMPILATION

### Build SSO
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet build
```

### Build + Clean
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet clean
dotnet build
```

### Build Release
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet build --configuration Release
```

---

## 🗄️ BASE DE DONNÉES

### Créer la DB
```bash
createdb onee_sso
```

### Appliquer les migrations
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet ef database update
```

### Créer une nouvelle migration
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet ef migrations add NomDeLaMigration
```

### Supprimer la dernière migration
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet ef migrations remove
```

### Reset complet de la DB
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet ef database drop
dotnet ef database update
```

---

## 🧪 TESTS

### Tester l'interface admin
```powershell
# Démarrer le SSO
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet run

# Ouvrir le navigateur
start http://localhost:5205/Dashboard
```

### Tester le flow SSO complet
```powershell
# Utiliser le script
cd c:\Users\XPS\source\repos\ONEE.SSO
.\START_TEST_RH.ps1

# Attendre 15 secondes puis ouvrir
start http://localhost:5173
```

### Tester seulement le SSO
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet run

# Tester le login
start http://localhost:5205/Login

# Tester le dashboard admin
start http://localhost:5205/Dashboard
```

---

## 📦 GESTION DES PACKAGES

### Restaurer les packages NuGet
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet restore
```

### Installer un package NuGet
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet add package NomDuPackage
```

### Mettre à jour les packages
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet restore --force
```

---

## 🔧 APPLICATIONS CLIENTES

### Gestion Personnel (RH)

#### Backend
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\gestion-personnel\backend
dotnet run
```

#### Frontend
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\gestion-personnel\frontend
npm install
npm run dev
```

### TIMS

#### Backend
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\tims\backend
dotnet run
```

#### Frontend
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\tims\frontend
npm install
npm run dev
```

### EAMS

#### Backend
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\eams\backend\ONEE.EAMS.API
dotnet run
```

#### Frontend
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\clients\eams\frontend
npm install
npm run dev
```

---

## 🌐 URLS D'ACCÈS

### SSO
- **Admin Dashboard**: http://localhost:5205/Dashboard
- **Login**: http://localhost:5205/Login
- **Authorize**: http://localhost:5205/Connect/Authorize
- **Token**: http://localhost:5205/connect/token (POST)
- **Logout**: http://localhost:5205/connect/logout

### Pages Admin
- **Utilisateurs**: http://localhost:5205/Users/Index
- **Rôles**: http://localhost:5205/Roles/Index
- **Applications**: http://localhost:5205/ClientApplications
- **Sessions**: http://localhost:5205/Sessions
- **Logs**: http://localhost:5205/AuditLogs
- **Paramètres**: http://localhost:5205/Settings

### Applications Clientes
- **Gestion Personnel**: http://localhost:5173
- **TIMS**: http://localhost:5175
- **EAMS**: http://localhost:5174

---

## 🔐 IDENTIFIANTS

### Comptes de Test
```
Email:    admin@onee.ma
Password: Admin@123
Rôles:    Admin, User
```

```
Email:    user@onee.ma
Password: User@123
Rôles:    User
```

```
Email:    manager@onee.ma
Password: Manager@123
Rôles:    Manager
```

---

## 🐛 DEBUGGING

### Voir les logs en temps réel
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet run --verbosity detailed
```

### Activer les logs EF Core
Dans `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### Voir les requêtes SQL
```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

---

## 📝 VÉRIFICATIONS

### Vérifier que le SSO fonctionne
```powershell
# Démarrer le SSO
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet run

# Dans la console, vérifier:
# ✅ "Application started"
# ✅ "== Seed Clients =="
# ✅ "== Seed Users =="
# ✅ Pas d'erreur rouge
```

### Vérifier la compilation
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet build

# Résultat attendu:
# ✅ "Générer a réussi dans X.Xs"
```

### Vérifier la base de données
```sql
-- Se connecter à PostgreSQL
psql -U postgres -d onee_sso

-- Lister les tables
\dt

-- Compter les utilisateurs
SELECT COUNT(*) FROM users;

-- Voir les clients
SELECT client_id, name, enabled FROM client_applications;
```

---

## 🔄 ARRÊT DES SERVICES

### Arrêter un processus en cours
Dans le terminal PowerShell qui exécute le service:
```
Ctrl + C
```

### Arrêter tous les processus .NET
```powershell
Get-Process -Name "dotnet" | Stop-Process -Force
```

### Arrêter tous les processus Node
```powershell
Get-Process -Name "node" | Stop-Process -Force
```

---

## 📊 MONITORING

### Voir les processus en cours
```powershell
# Processus .NET
Get-Process -Name "dotnet"

# Processus Node
Get-Process -Name "node"

# Ports utilisés
netstat -ano | findstr "5205"
netstat -ano | findstr "5173"
netstat -ano | findstr "5291"
```

### Libérer un port occupé
```powershell
# Trouver le PID du processus
netstat -ano | findstr "5205"

# Tuer le processus
taskkill /PID <PID> /F
```

---

## 🔧 CONFIGURATION

### Modifier le port du SSO
Dans `Properties/launchSettings.json`:
```json
{
  "profiles": {
    "http": {
      "applicationUrl": "http://localhost:5205"
    }
  }
}
```

### Modifier la connection string
Dans `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=onee_sso;Username=postgres;Password=your_password"
  }
}
```

### Modifier la durée des tokens
Dans `appsettings.json`:
```json
{
  "Jwt": {
    "AccessTokenExpirationMinutes": 60
  }
}
```

---

## 📦 PUBLICATION

### Publier en mode Release
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet publish --configuration Release --output ./publish
```

### Créer un executable
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet publish --configuration Release --runtime win-x64 --self-contained true
```

---

## 🎯 RACCOURCIS UTILES

### Rebuild complet
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet clean
dotnet restore
dotnet build
```

### Test rapide
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO
.\START_TEST_RH.ps1
```

### Ouvrir dans VS Code
```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO
code .
```

---

## 📚 DOCUMENTATION

### Lire les guides
```powershell
# Guide de test rapide
notepad GUIDE_TEST_RAPIDE.md

# État actuel du projet
notepad ETAT_ACTUEL.md

# README complet
notepad README_FINAL.md

# Ce qui reste à faire
notepad CE_QUI_RESTE_A_FAIRE.md
```

---

## ✅ CHECKLIST AVANT DÉMO

```powershell
# 1. Build réussi
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet build

# 2. Démarrer les services
.\START_TEST_RH.ps1

# 3. Attendre 15 secondes

# 4. Tester le flow
start http://localhost:5173

# 5. Login
# Email: admin@onee.ma
# Password: Admin@123

# 6. Vérifier dashboard reste stable
```

---

## 🏁 COMMANDE ULTIME (Tout-en-un)

```powershell
# Aller dans le dossier du projet
cd c:\Users\XPS\source\repos\ONEE.SSO

# Build
cd src\ONEE.SSO.API
dotnet clean
dotnet build
cd ..\..

# Lancer les tests
.\START_TEST_RH.ps1
```

---

**Dernière mise à jour**: 24 Août 2026  
**Version**: 1.0.0  
**Status**: ✅ Prêt pour la soutenance
