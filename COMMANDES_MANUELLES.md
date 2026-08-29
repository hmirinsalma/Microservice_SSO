# 📋 COMMANDES MANUELLES - LANCEMENT DES SERVEURS

Si le script PowerShell ne fonctionne pas, utilisez ces commandes manuelles.

---

## 🚀 OPTION 1 : SCRIPT AUTOMATIQUE (RECOMMANDÉ)

```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO
.\LANCER_TOUS_LES_SERVEURS.ps1
```

---

## 🔧 OPTION 2 : COMMANDES MANUELLES (7 TERMINAUX)

Ouvrir **7 terminaux PowerShell** (ou CMD) et exécuter ces commandes :

---

### 📟 TERMINAL 1 : Serveur SSO (Port 5205)

```powershell
cd c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API
dotnet run
```

**Attendu** : `Now listening on: http://localhost:5205`

---

### 📟 TERMINAL 2 : Backend Gestion Personnel (Port 5291)

```powershell
cd "c:\Users\XPS\Desktop\Gestion du Prsonnel\backend\GestionPersonnel.API"
dotnet run
```

**Attendu** : `Now listening on: http://localhost:5291`

---

### 📟 TERMINAL 3 : Backend TIMS (Port 5115)

```powershell
cd "c:\Users\XPS\Desktop\gestion des interventions\backend\TIMS.API"
dotnet run
```

**Attendu** : `Now listening on: http://localhost:5115`

---

### 📟 TERMINAL 4 : Backend EAMS (Port 5137)

```powershell
cd "c:\Users\XPS\Desktop\gestion des equipements\backend\ONEE.EAMS.API"
dotnet run
```

**Attendu** : `Now listening on: http://localhost:5137`

---

### 📟 TERMINAL 5 : Frontend Gestion Personnel (Port 5173)

```powershell
cd "c:\Users\XPS\Desktop\Gestion du Prsonnel\frontend"
npm run dev
```

**Attendu** : `Local: http://localhost:5173`

---

### 📟 TERMINAL 6 : Frontend TIMS (Port auto)

```powershell
cd "c:\Users\XPS\Desktop\gestion des interventions\frontend"
npm run dev
```

**Attendu** : `Local: http://localhost:XXXX` (noter le port)

---

### 📟 TERMINAL 7 : Frontend EAMS (Port auto)

```powershell
cd "c:\Users\XPS\Desktop\gestion des equipements\frontend"
npm run dev
```

**Attendu** : `Local: http://localhost:XXXX` (noter le port)

---

## 📊 VÉRIFICATION RAPIDE

Une fois tous les serveurs lancés, ouvrir ces URLs dans un navigateur :

### Swagger UI (Backends)
- **SSO** : http://localhost:5205/swagger
- **RH** : http://localhost:5291/swagger
- **TIMS** : http://localhost:5115/swagger
- **EAMS** : http://localhost:5137/swagger

### Frontends
- **RH** : http://localhost:5173
- **TIMS** : http://localhost:XXXX (le port affiché dans le terminal 6)
- **EAMS** : http://localhost:XXXX (le port affiché dans le terminal 7)

---

## ⚠️ EN CAS DE PROBLÈME

### Problème : "Port already in use"

**Solution 1** : Trouver et arrêter le processus
```powershell
# Trouver le processus sur un port
netstat -ano | findstr :5205

# Arrêter le processus (remplacer PID par le numéro trouvé)
taskkill /PID [PID] /F
```

**Solution 2** : Changer le port
- Pour les backends : Modifier `Properties/launchSettings.json`
- Pour les frontends : Vite choisira automatiquement un port libre

---

### Problème : "dotnet : commande introuvable"

**Solution** : Installer .NET 9 SDK
```
https://dotnet.microsoft.com/download/dotnet/9.0
```

---

### Problème : "npm : commande introuvable"

**Solution** : Installer Node.js
```
https://nodejs.org/
```

---

### Problème : "Build failed" sur un backend

**Solution** : Restaurer les packages
```powershell
cd [dossier du backend]
dotnet restore
dotnet build
```

---

### Problème : "npm ERR!" sur un frontend

**Solution** : Réinstaller les dépendances
```powershell
cd [dossier du frontend]
rm -r node_modules
rm package-lock.json
npm install
npm run dev
```

---

## 🛑 ARRÊTER TOUS LES SERVEURS

**Méthode 1** : Fermer toutes les fenêtres PowerShell/CMD

**Méthode 2** : Dans chaque terminal, appuyer sur `Ctrl+C`

---

## 📖 SUITE DES TESTS

Une fois tous les serveurs lancés, suivre le guide :
```
GUIDE_TESTS_E2E.md
```

---

## 🎯 RÉSUMÉ DES PORTS

| Service | Port | URL |
|---------|------|-----|
| SSO | 5205 | http://localhost:5205 |
| Backend RH | 5291 | http://localhost:5291 |
| Backend TIMS | 5115 | http://localhost:5115 |
| Backend EAMS | 5137 | http://localhost:5137 |
| Frontend RH | 5173 | http://localhost:5173 |
| Frontend TIMS | Auto | Voir terminal 6 |
| Frontend EAMS | Auto | Voir terminal 7 |

---

## 🔑 IDENTIFIANTS DE TEST

```
Email    : admin@onee.ma
Password : Admin@123
```

---

**BON TEST ! 🚀**
