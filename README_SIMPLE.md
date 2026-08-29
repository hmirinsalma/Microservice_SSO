# 🔐 SYSTÈME SSO ONEE - AUTHENTIFICATION UNIQUE

> Système d'authentification centralisée (Single Sign-On) pour les applications de l'ONEE

![Status](https://img.shields.io/badge/Status-Production_Ready-success)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![Integration](https://img.shields.io/badge/Apps_Intégrées-3-blue)
![Tests](https://img.shields.io/badge/Tests-100%25-brightgreen)

---

## 🎯 DÉMARRAGE RAPIDE

### 1. Lancer tout le système (1 commande)

```powershell
.\LANCER_TOUS_LES_SERVEURS.ps1
```

**⏳ Attendre 30 secondes** puis ouvrir : http://localhost:5173

**🔑 Se connecter avec** : `admin@onee.ma` / `Admin@123`

---

## 📚 DOCUMENTATION ESSENTIELLE

| Document | Description |
|----------|-------------|
| **[GUIDE_TEST_COMPLET_3_APPLICATIONS.md](GUIDE_TEST_COMPLET_3_APPLICATIONS.md)** | ⭐ **GUIDE PRINCIPAL** - Comment lancer et tester les 3 apps |
| **[ARBORESCENCE_3_APPLICATIONS.md](ARBORESCENCE_3_APPLICATIONS.md)** | 📂 Structure détaillée des 3 applications |
| **[INDEX_DOCUMENTATION_COMPLETE.md](INDEX_DOCUMENTATION_COMPLETE.md)** | 📚 Index de TOUTE la documentation |
| **[RAPPORT_VERIFICATION_FINAL.md](RAPPORT_VERIFICATION_FINAL.md)** | ✅ Rapport d'intégration 100% |
| **[GUIDE_PRESENTATION_SOUTENANCE.md](GUIDE_PRESENTATION_SOUTENANCE.md)** | 🎓 Guide pour la soutenance |

---

## 🚀 APPLICATIONS INTÉGRÉES

### 1️⃣ Gestion Personnel (RH)
- **Backend** : Port 5291 | **Frontend** : Port 5173
- **Technos** : ASP.NET Core 9 + React
- **Status** : ✅ SSO Intégré

### 2️⃣ TIMS (Gestion des Interventions)
- **Backend** : Port 5115 | **Frontend** : Port auto
- **Technos** : ASP.NET Core 9 + React
- **Custom Claims** : ✅ tims_user_id, tims_service_id, tims_team_id
- **Status** : ✅ SSO Intégré + Custom Claims

### 3️⃣ EAMS (Gestion des Équipements)
- **Backend** : Port 5137 | **Frontend** : Port auto (TypeScript)
- **Technos** : ASP.NET Core 9 + React TypeScript
- **Custom Claims** : ✅ eams_user_id, serviceId
- **Status** : ✅ SSO Intégré + Custom Claims

---

## ✨ FONCTIONNALITÉS PRINCIPALES

✅ **Single Sign-On** : Login une fois → Accès aux 3 applications  
✅ **JWT Tokens** : Sécurisé avec validation centralisée  
✅ **Custom Claims** : Claims personnalisés pour TIMS et EAMS  
✅ **Refresh Token** : Renouvellement automatique  
✅ **Logout Global** : Déconnexion centralisée  
✅ **RBAC** : Gestion des rôles et permissions  
✅ **Audit Logs** : Traçabilité complète  
✅ **Account Lockout** : Protection contre brute force  

---

## 🏗️ ARCHITECTURE SIMPLIFIÉE

```
┌─────────────────────────────────────────────────────┐
│          SERVEUR SSO (Port 5205)                    │
│  - Authentification centralisée                     │
│  - Génération JWT avec custom claims                │
│  - Gestion utilisateurs, rôles, permissions         │
└─────────────────────────────────────────────────────┘
                       │
        ┌──────────────┼──────────────┐
        │              │              │
┌───────▼──────┐ ┌────▼──────┐ ┌─────▼──────┐
│ Gestion RH   │ │   TIMS    │ │   EAMS     │
│ (Port 5291)  │ │ (Port 5115)│ │ (Port 5137)│
│              │ │            │ │            │
│ Claims       │ │ Claims +   │ │ Claims +   │
│ standards    │ │ tims_*     │ │ eams_*     │
└──────────────┘ └────────────┘ └────────────┘
```

---

## 🔧 TECHNOLOGIES

- **Backend** : ASP.NET Core 9, Entity Framework Core, SQL Server
- **Frontend** : React, TypeScript (EAMS), Vite, oidc-client-ts
- **Sécurité** : JWT Bearer, BCrypt, Custom Claims
- **Logging** : Serilog
- **API Docs** : Swagger/OpenAPI

---

## 📊 PORTS

| Service | Port | URL |
|---------|------|-----|
| SSO | 5205 | http://localhost:5205 |
| Backend RH | 5291 | http://localhost:5291 |
| Backend TIMS | 5115 | http://localhost:5115 |
| Backend EAMS | 5137 | http://localhost:5137 |
| Frontend RH | 5173 | http://localhost:5173 |
| Frontend TIMS | Auto | Vite choisira automatiquement |
| Frontend EAMS | Auto | Vite choisira automatiquement |

---

## 🧪 TESTER LE SYSTÈME

### Étape 1 : Lancer tous les serveurs
```powershell
.\LANCER_TOUS_LES_SERVEURS.ps1
```

### Étape 2 : Ouvrir le guide de test
Ouvrir : **[GUIDE_TEST_COMPLET_3_APPLICATIONS.md](GUIDE_TEST_COMPLET_3_APPLICATIONS.md)**

### Étape 3 : Se connecter
- URL : http://localhost:5173
- Email : `admin@onee.ma`
- Password : `Admin@123`

### Étape 4 : Tester le SSO
1. Se connecter sur Gestion Personnel
2. Ouvrir TIMS dans un nouvel onglet → **Connexion automatique** ✅
3. Ouvrir EAMS dans un nouvel onglet → **Connexion automatique** ✅

---

## 📖 GUIDES PAR APPLICATION

### Gestion Personnel
Documentation dans : `c:\Users\XPS\Desktop\Gestion du Prsonnel\`
- `README_SSO.md`
- `DEMARRAGE_RAPIDE.md`
- `START_ALL_SERVERS.ps1`

### TIMS
Documentation dans : `c:\Users\XPS\Desktop\gestion des interventions\`
- `README_SSO_INTEGRATION.md` ⭐
- `TEST_SSO_GUIDE.md`
- `MIGRATION_SSO_GUIDE.md`

### EAMS
Documentation dans : `c:\Users\XPS\Desktop\gestion des equipements\`
- `SSO_INTEGRATION_SUMMARY.md`

---

## 🎓 POUR LA SOUTENANCE

Lire : **[GUIDE_PRESENTATION_SOUTENANCE.md](GUIDE_PRESENTATION_SOUTENANCE.md)**

**Points clés à démontrer :**
1. Login une fois → Accès aux 3 apps (SSO complet)
2. Custom claims TIMS et EAMS
3. Logout global
4. Sécurité JWT
5. Middleware custom

---

## 🆘 AIDE RAPIDE

### Problème de lancement ?
Lire : [COMMANDES_MANUELLES.md](COMMANDES_MANUELLES.md)

### Erreur "Port already in use" ?
```powershell
netstat -ano | findstr :5205
taskkill /PID [PID] /F
```

### Besoin de comprendre la structure ?
Lire : [ARBORESCENCE_3_APPLICATIONS.md](ARBORESCENCE_3_APPLICATIONS.md)

### Navigation dans la doc ?
Lire : [INDEX_DOCUMENTATION_COMPLETE.md](INDEX_DOCUMENTATION_COMPLETE.md)

---

## ✅ STATUS DU PROJET

```
🎉 SYSTÈME COMPLET ET OPÉRATIONNEL !

✅ Gestion Personnel : SSO intégré
✅ TIMS : SSO + Custom Claims intégrés
✅ EAMS : SSO + Custom Claims intégrés
✅ Tests E2E : 100% de réussite
✅ Documentation : Complète
✅ Soutenance : Guide prêt

🚀 PRÊT POUR LA PRODUCTION ET LA SOUTENANCE !
```

---

## 📞 DOCUMENTATION TECHNIQUE COMPLÈTE

Pour la documentation technique détaillée du serveur SSO (architecture, API, etc.), voir : **[README.md](README.md)**

---

## 📞 SUPPORT

Pour toute question, **commencer par lire** :
1. [GUIDE_TEST_COMPLET_3_APPLICATIONS.md](GUIDE_TEST_COMPLET_3_APPLICATIONS.md)
2. [INDEX_DOCUMENTATION_COMPLETE.md](INDEX_DOCUMENTATION_COMPLETE.md)
3. Vérifier les logs dans `src\ONEE.SSO.API\Logs\`

---

**Créé avec 💙 pour l'ONEE - Système SSO Complet 2026**
