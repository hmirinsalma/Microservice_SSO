# 🧪 GUIDE DE TEST IMMÉDIAT - SYSTÈME SSO

**Tous vos serveurs sont lancés !** 🎉

---

## 📊 VOS PORTS ACTUELS

| Application | URL |
|-------------|-----|
| 🔐 **SSO** | http://localhost:5205 |
| 📊 **Backend RH** | http://localhost:5291 |
| 🔧 **Backend TIMS** | http://localhost:5115 |
| ⚙️ **Backend EAMS** | http://localhost:5137 |
| 🖥️ **Frontend RH** | http://localhost:5174 |
| 🖥️ **Frontend TIMS** | http://localhost:5175 |
| 🖥️ **Frontend EAMS** | http://localhost:5173 |

---

## ✅ ÉTAPE 1 : VÉRIFIER LES SWAGGER

Ouvrir ces URLs dans un navigateur :

1. **SSO** : http://localhost:5205/swagger
2. **Backend RH** : http://localhost:5291/swagger
3. **Backend TIMS** : http://localhost:5115/swagger
4. **Backend EAMS** : http://localhost:5137/swagger

**✅ Si les 4 pages Swagger s'affichent → Parfait !**

---

## 🧪 ÉTAPE 2 : TESTER LE SSO - GESTION PERSONNEL

### 1. Ouvrir l'application
```
http://localhost:5174
```

### 2. Se connecter avec SSO

- Cliquer sur **"Se connecter avec ONEE SSO"**
- Vous serez redirigé vers le serveur SSO

### 3. Saisir les identifiants

```
Email    : admin@onee.ma
Password : Admin@123
```

### 4. Cliquer sur "Se connecter"

**✅ Attendu** : 
- Redirection vers le Dashboard
- Utilisateur connecté
- Nom affiché dans l'interface

---

## 🧪 ÉTAPE 3 : TESTER LE SSO - TIMS (SANS RE-LOGIN)

### 1. Ouvrir un NOUVEL ONGLET (même navigateur)
```
http://localhost:5175
```

### 2. Observer le comportement

**✅ Attendu** : 
- **Connexion automatique** (pas de re-saisie du mot de passe)
- Dashboard TIMS affiché
- C'est la magie du SSO ! 🎉

### 3. Vérifier les custom claims TIMS

1. Appuyer sur **F12** (DevTools)
2. Aller dans l'onglet **Console**
3. Chercher les logs avec les custom claims

**✅ Attendu** : Voir ces 3 claims TIMS
```javascript
{
  "tims_user_id": "valeur",
  "tims_service_id": "valeur",
  "tims_team_id": "valeur"
}
```

---

## 🧪 ÉTAPE 4 : TESTER LE SSO - EAMS (SANS RE-LOGIN)

### 1. Ouvrir un NOUVEL ONGLET (même navigateur)
```
http://localhost:5173
```

### 2. Observer le comportement

**✅ Attendu** : 
- **Connexion automatique** encore une fois !
- Dashboard EAMS affiché
- SSO fonctionne sur les 3 apps ! 🎊

### 3. Vérifier les custom claims EAMS

1. Appuyer sur **F12** (DevTools)
2. Aller dans l'onglet **Console**
3. Chercher les logs avec les custom claims

**✅ Attendu** : Voir ces 2 claims EAMS
```javascript
{
  "eams_user_id": "valeur",
  "serviceId": "valeur"
}
```

---

## 🧪 ÉTAPE 5 : TESTER LE LOGOUT GLOBAL

### 1. Retourner sur l'onglet Gestion Personnel
```
http://localhost:5174
```

### 2. Se déconnecter

Cliquer sur **"Déconnexion"**

### 3. Vérifier TIMS

- Retourner sur l'onglet TIMS (http://localhost:5175)
- Recharger la page (F5)

**✅ Attendu** : Redirection vers la page de login

### 4. Vérifier EAMS

- Retourner sur l'onglet EAMS (http://localhost:5173)
- Recharger la page (F5)

**✅ Attendu** : Redirection vers la page de login

**🎉 RÉSULTAT** : Logout une fois = déconnexion de toutes les apps !

---

## 🎯 RÉSUMÉ DU TEST

Si tous ces tests passent :

✅ **Login une fois → Accès aux 3 applications automatiquement**  
✅ **Custom claims TIMS fonctionnent** (3 claims)  
✅ **Custom claims EAMS fonctionnent** (2 claims)  
✅ **Logout global fonctionne** (déconnexion centralisée)

---

## 🎉 SI TOUS LES TESTS PASSENT

```
═══════════════════════════════════════════════════════════
🎊 FÉLICITATIONS ! VOTRE SYSTÈME SSO EST OPÉRATIONNEL ! 🎊
═══════════════════════════════════════════════════════════

✅ 7 serveurs lancés et fonctionnels
✅ SSO complet entre 3 applications
✅ Custom claims TIMS et EAMS validés
✅ Logout global opérationnel
✅ Sécurité JWT centralisée

🚀 SYSTÈME PRÊT POUR LA PRODUCTION ET LA SOUTENANCE ! 🎉
```

---

## 📸 POINTS À MONTRER POUR LA SOUTENANCE

1. **Les 7 terminaux PowerShell** avec les serveurs actifs
2. **Les 4 pages Swagger** des backends
3. **Le flow de connexion** : Login une fois → Accès aux 3 apps
4. **Les custom claims dans la console** (F12)
5. **Le logout global** : Déconnexion partout

---

## 🔑 IDENTIFIANTS

```
Email    : admin@onee.ma
Password : Admin@123
```

---

## 🆘 EN CAS DE PROBLÈME

### Page blanche ou erreur 404
➡️ Vérifier que le serveur correspondant est bien lancé

### "Token invalide" ou "Unauthorized"
➡️ Vérifier que le SSO (port 5205) est bien lancé

### Pas de connexion automatique
➡️ Vider le cache du navigateur ou utiliser mode navigation privée

### Custom claims non visibles
➡️ Vérifier dans Console (F12) et chercher les logs de l'auth service

---

**🚀 BON TEST ! VOUS ÊTES PRESQUE À LA FIN ! 🎉**
