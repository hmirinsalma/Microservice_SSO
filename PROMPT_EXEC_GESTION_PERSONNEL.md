# 🚀 PROMPT D'EXÉCUTION - INTÉGRATION SSO GESTION PERSONNEL

## CONTEXTE DU PROJET

Je développe **ONEE.SSO**, un serveur SSO complet en ASP.NET Core 9 qui fonctionne déjà (tests backend 100% réussis). Le serveur SSO tourne sur **http://localhost:5205**.

J'ai 3 applications clientes existantes à intégrer. Tu vas m'aider à intégrer la première : **Gestion Personnel (RH)**.

---

## INFORMATIONS DE L'APPLICATION GESTION PERSONNEL

- **Framework Frontend** : React 19
- **Port Frontend** : http://localhost:5173
- **Backend** : ASP.NET Core
- **Port Backend** : http://localhost:5291
- **Dossier Projet Frontend** : `c:\Users\XPS\Desktop\gestion-rh-frontend`
- **Dossier Projet Backend** : `c:\Users\XPS\Desktop\gestion-rh-backend`

---

## INFORMATIONS DU SERVEUR SSO

- **URL SSO** : http://localhost:5205
- **ClientId** : `gestion-personnel`
- **ClientSecret** : `secret-gestion-personnel-2024`
- **Scopes** : `openid profile email roles offline_access gestion-personnel`
- **Endpoints OIDC** :
  - Authorization : `http://localhost:5205/connect/authorize`
  - Token : `http://localhost:5205/connect/token`
  - Userinfo : `http://localhost:5205/api/auth/userinfo`
  - Logout : `http://localhost:5205/connect/logout`
  - Discovery : `http://localhost:5205/.well-known/openid-configuration`

---

## 🎯 TON TRAVAIL : INTÉGRER LE SSO DANS L'APPLICATION GESTION PERSONNEL

### ÉTAPE 1 : INSTALLATION DES PACKAGES NPM (Frontend)

Dans le dossier frontend, exécute :
```bash
cd c:\Users\XPS\Desktop\gestion-rh-frontend
npm install oidc-client-ts react-router-dom
```

---

### ÉTAPE 2 : CRÉER LA CONFIGURATION OIDC

Crée le fichier `src/auth/authConfig.js` avec ce contenu exact :

```javascript
import { UserManager, WebStorageStateStore } from 'oidc-client-ts';

const AUTHORITY = 'http://localhost:5205';
const CLIENT_ID = 'gestion-personnel';
const CLIENT_SECRET = 'secret-gestion-personnel-2024';
const REDIRECT_URI = 'http://localhost:5173/callback';
const POST_LOGOUT_REDIRECT_URI = 'http://localhost:5173';
const SILENT_REDIRECT_URI = 'http://localhost:5173/silent-renew.html';

export const oidcConfig = {
  authority: AUTHORITY,
  client_id: CLIENT_ID,
  client_secret: CLIENT_SECRET,
  redirect_uri: REDIRECT_URI,
  post_logout_redirect_uri: POST_LOGOUT_REDIRECT_URI,
  silent_redirect_uri: SILENT_REDIRECT_URI,
  response_type: 'code',
  scope: 'openid profile email roles offline_access gestion-personnel',
  automaticSilentRenew: true,
  loadUserInfo: true,
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  metadata: {
    issuer: 'ONEE.SSO',
    authorization_endpoint: `${AUTHORITY}/connect/authorize`,
    token_endpoint: `${AUTHORITY}/connect/token`,
    userinfo_endpoint: `${AUTHORITY}/api/auth/userinfo`,
    end_session_endpoint: `${AUTHORITY}/connect/logout`,
    jwks_uri: `${AUTHORITY}/.well-known/jwks.json`,
  }
};

export const userManager = new UserManager(oidcConfig);

userManager.events.addUserLoaded((user) => {
  console.log('✅ User loaded:', user.profile);
});

userManager.events.addUserUnloaded(() => {
  console.log('🚪 User logged out');
});

userManager.events.addAccessTokenExpired(() => {
  console.log('⏱️ Access token expired');
  userManager.signinSilent();
});

userManager.events.addSilentRenewError((error) => {
  console.error('❌ Silent renew error:', error);
});
```

---

### ÉTAPE 3 : CRÉER LE SERVICE D'AUTHENTIFICATION

Crée le fichier `src/auth/authService.js` avec ce contenu exact :

```javascript
import { userManager } from './authConfig';

class AuthService {
  login() {
    return userManager.signinRedirect();
  }

  async completeLogin() {
    try {
      const user = await userManager.signinRedirectCallback();
      return user;
    } catch (error) {
      console.error('❌ Login callback error:', error);
      throw error;
    }
  }

  logout() {
    return userManager.signoutRedirect();
  }

  async getUser() {
    return await userManager.getUser();
  }

  async isAuthenticated() {
    const user = await this.getUser();
    return user !== null && !user.expired;
  }

  async getAccessToken() {
    const user = await this.getUser();
    return user?.access_token;
  }

  async getUserProfile() {
    const user = await this.getUser();
    return user?.profile;
  }

  async getUserRoles() {
    const user = await this.getUser();
    return user?.profile?.roles || [];
  }

  async getUserPermissions() {
    const user = await this.getUser();
    return user?.profile?.permissions || [];
  }

  async hasRole(role) {
    const roles = await this.getUserRoles();
    return roles.includes(role);
  }

  async hasPermission(permission) {
    const permissions = await this.getUserPermissions();
    return permissions.includes(permission);
  }
}

export default new AuthService();
```

---

### ÉTAPE 4 : CRÉER LA PAGE DE LOGIN

Crée le fichier `src/pages/Login.jsx` :

```jsx
import React from 'react';
import authService from '../auth/authService';

const Login = () => {
  const handleLogin = () => {
    authService.login();
  };

  return (
    <div style={{ 
      display: 'flex', 
      flexDirection: 'column', 
      alignItems: 'center', 
      justifyContent: 'center', 
      height: '100vh' 
    }}>
      <h1>Gestion Personnel - ONEE</h1>
      <p>Connectez-vous avec votre compte ONEE SSO</p>
      <button 
        onClick={handleLogin}
        style={{
          padding: '12px 24px',
          fontSize: '16px',
          backgroundColor: '#0066cc',
          color: 'white',
          border: 'none',
          borderRadius: '4px',
          cursor: 'pointer'
        }}
      >
        🔐 Se connecter avec ONEE SSO
      </button>
    </div>
  );
};

export default Login;
```

---

### ÉTAPE 5 : CRÉER LA PAGE DE CALLBACK

Crée le fichier `src/pages/Callback.jsx` :

```jsx
import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import authService from '../auth/authService';

const Callback = () => {
  const navigate = useNavigate();

  useEffect(() => {
    const completeLogin = async () => {
      try {
        await authService.completeLogin();
        navigate('/dashboard');
      } catch (error) {
        console.error('❌ Erreur lors du callback:', error);
        navigate('/login');
      }
    };

    completeLogin();
  }, [navigate]);

  return (
    <div style={{ textAlign: 'center', marginTop: '100px' }}>
      <h2>Authentification en cours...</h2>
      <p>Veuillez patienter</p>
    </div>
  );
};

export default Callback;
```

---

### ÉTAPE 6 : CRÉER LE SILENT RENEW HTML

Crée le fichier `public/silent-renew.html` :

```html
<!DOCTYPE html>
<html>
<head>
  <title>Silent Renew</title>
</head>
<body>
  <script src="https://unpkg.com/oidc-client-ts@2.4.0/dist/browser/oidc-client-ts.min.js"></script>
  <script>
    new oidc.UserManager({
      userStore: new oidc.WebStorageStateStore({ store: window.localStorage })
    }).signinSilentCallback();
  </script>
</body>
</html>
```

---

### ÉTAPE 7 : CRÉER LE PROTECTED ROUTE

Crée le fichier `src/components/ProtectedRoute.jsx` :

```jsx
import React, { useEffect, useState } from 'react';
import { Navigate } from 'react-router-dom';
import authService from '../auth/authService';

const ProtectedRoute = ({ children, requiredRole, requiredPermission }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(null);
  const [hasAccess, setHasAccess] = useState(true);

  useEffect(() => {
    const checkAuth = async () => {
      const authenticated = await authService.isAuthenticated();
      setIsAuthenticated(authenticated);

      if (authenticated) {
        if (requiredRole) {
          const hasRole = await authService.hasRole(requiredRole);
          setHasAccess(hasRole);
        }

        if (requiredPermission) {
          const hasPermission = await authService.hasPermission(requiredPermission);
          setHasAccess(hasPermission);
        }
      }
    };

    checkAuth();
  }, [requiredRole, requiredPermission]);

  if (isAuthenticated === null) {
    return <div>Chargement...</div>;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (!hasAccess) {
    return <div>⛔ Accès refusé - Permissions insuffisantes</div>;
  }

  return children;
};

export default ProtectedRoute;
```

---

### ÉTAPE 8 : CRÉER L'AXIOS INTERCEPTOR

Crée le fichier `src/api/axiosConfig.js` :

```javascript
import axios from 'axios';
import authService from '../auth/authService';

const API_BASE_URL = 'http://localhost:5291/api';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

apiClient.interceptors.request.use(
  async (config) => {
    const token = await authService.getAccessToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      console.warn('⚠️ Token expiré, redirection vers login');
      await authService.logout();
    }
    return Promise.reject(error);
  }
);

export default apiClient;
```

---

### ÉTAPE 9 : MODIFIER APP.JSX

Modifie le fichier `src/App.jsx` pour ajouter les routes :

```jsx
import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Callback from './pages/Callback';
import Dashboard from './pages/Dashboard'; // Ta page existante
import ProtectedRoute from './components/ProtectedRoute';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/callback" element={<Callback />} />
        
        <Route 
          path="/dashboard" 
          element={
            <ProtectedRoute>
              <Dashboard />
            </ProtectedRoute>
          } 
        />

        <Route path="/" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
```

---

### ÉTAPE 10 : CONFIGURATION BACKEND ASP.NET CORE

#### 10.1 Installation Package
Dans le backend, exécute :
```bash
cd c:\Users\XPS\Desktop\gestion-rh-backend
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

#### 10.2 Modifier appsettings.json
Ajoute cette section dans `appsettings.json` :

```json
{
  "JwtSettings": {
    "SecretKey": "VotreCléSecrèteSuperSecure2024!MinimumLongueur32Caractères",
    "Issuer": "ONEE.SSO",
    "Audience": "ONEE.Applications",
    "ExpirationMinutes": 60
  }
}
```

#### 10.3 Modifier Program.cs
Ajoute la configuration JWT dans `Program.cs` :

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuration JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

#### 10.4 Protéger les Controllers
Ajoute `[Authorize]` sur tes controllers :

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetEmployees()
    {
        var userId = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;
        var roles = User.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                        .Select(c => c.Value).ToList();

        return Ok(new { userId, email, roles });
    }
}
```

---

## 🧪 ÉTAPE 11 : TESTS

### Test 1 : Lancer les 3 serveurs
1. SSO : `dotnet run --project c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API`
2. Backend RH : `cd c:\Users\XPS\Desktop\gestion-rh-backend && dotnet run`
3. Frontend RH : `cd c:\Users\XPS\Desktop\gestion-rh-frontend && npm run dev`

### Test 2 : Login
1. Aller sur http://localhost:5173
2. Cliquer "Se connecter avec ONEE SSO"
3. Login : `admin@onee.ma` / `Admin@123`
4. Vérifier redirection vers `/dashboard`

### Test 3 : API Call
1. Dans Dashboard, faire un appel API
2. Vérifier que le token est dans les headers
3. Vérifier que le backend retourne les données

### Test 4 : Logout
1. Cliquer sur déconnexion
2. Vérifier la redirection

---

## ✅ RÉSULTAT ATTENDU

✅ Login sur http://localhost:5173 → Redirection vers SSO  
✅ Login SSO → Callback → Dashboard  
✅ Toutes les requêtes API incluent le token Bearer  
✅ Backend valide le token et retourne les données  
✅ Logout révoque la session  

---

## 🎯 INSTRUCTIONS POUR TOI (KIRO)

**EXÉCUTE TOUTES CES ÉTAPES DANS L'ORDRE :**

1. ✅ Installe les packages npm dans le frontend
2. ✅ Crée tous les fichiers React listés ci-dessus
3. ✅ Installe le package NuGet dans le backend
4. ✅ Modifie `appsettings.json` et `Program.cs` du backend
5. ✅ Valide que tous les fichiers sont créés
6. ✅ Affiche un résumé de ce qui a été fait

**NE POSE PAS DE QUESTIONS, EXÉCUTE DIRECTEMENT !**
