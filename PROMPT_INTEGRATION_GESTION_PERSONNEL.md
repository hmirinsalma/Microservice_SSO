# 🔐 PROMPT D'INTÉGRATION SSO - GESTION PERSONNEL (RH)

## 📋 INFORMATIONS DE L'APPLICATION

- **Framework** : React 19
- **Port Frontend** : http://localhost:5173
- **Backend** : ASP.NET Core
- **Port Backend** : http://localhost:5291
- **SSO Server** : http://localhost:5205
- **ClientId** : `gestion-personnel`
- **Scopes** : `openid profile email roles offline_access gestion-personnel`

---

## 🚀 ÉTAPE 1 : INSTALLATION DES DÉPENDANCES

### Frontend React

```bash
npm install oidc-client-ts react-router-dom
```

---

## 🔧 ÉTAPE 2 : CONFIGURATION OIDC (Frontend)

Créer le fichier `src/auth/authConfig.js` :

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

// Event listeners
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

## 🔐 ÉTAPE 3 : SERVICE D'AUTHENTIFICATION

Créer le fichier `src/auth/authService.js` :

```javascript
import { userManager } from './authConfig';

class AuthService {
  // Login - Redirect to SSO
  login() {
    return userManager.signinRedirect();
  }

  // Handle callback from SSO
  async completeLogin() {
    try {
      const user = await userManager.signinRedirectCallback();
      return user;
    } catch (error) {
      console.error('❌ Login callback error:', error);
      throw error;
    }
  }

  // Logout
  logout() {
    return userManager.signoutRedirect();
  }

  // Get current user
  async getUser() {
    return await userManager.getUser();
  }

  // Check if user is authenticated
  async isAuthenticated() {
    const user = await this.getUser();
    return user !== null && !user.expired;
  }

  // Get access token
  async getAccessToken() {
    const user = await this.getUser();
    return user?.access_token;
  }

  // Get user profile
  async getUserProfile() {
    const user = await this.getUser();
    return user?.profile;
  }

  // Get user roles
  async getUserRoles() {
    const user = await this.getUser();
    return user?.profile?.roles || [];
  }

  // Get user permissions
  async getUserPermissions() {
    const user = await this.getUser();
    return user?.profile?.permissions || [];
  }

  // Check if user has role
  async hasRole(role) {
    const roles = await this.getUserRoles();
    return roles.includes(role);
  }

  // Check if user has permission
  async hasPermission(permission) {
    const permissions = await this.getUserPermissions();
    return permissions.includes(permission);
  }
}

export default new AuthService();
```

---

## 📄 ÉTAPE 4 : PAGES REACT

### 4.1 Page de Login

Créer `src/pages/Login.jsx` :

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

### 4.2 Page de Callback

Créer `src/pages/Callback.jsx` :

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
        navigate('/dashboard'); // Rediriger vers votre page principale
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

### 4.3 Silent Renew HTML

Créer `public/silent-renew.html` :

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

## 🛡️ ÉTAPE 5 : PROTECTED ROUTE

Créer `src/components/ProtectedRoute.jsx` :

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
        // Check role if required
        if (requiredRole) {
          const hasRole = await authService.hasRole(requiredRole);
          setHasAccess(hasRole);
        }

        // Check permission if required
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

## 🔄 ÉTAPE 6 : AXIOS INTERCEPTOR

Créer `src/api/axiosConfig.js` :

```javascript
import axios from 'axios';
import authService from '../auth/authService';

const API_BASE_URL = 'http://localhost:5291/api'; // Votre backend

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Request interceptor - Ajouter le token automatiquement
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

// Response interceptor - Gérer les erreurs 401
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

## 🗺️ ÉTAPE 7 : CONFIGURATION DES ROUTES

Modifier `src/App.jsx` :

```jsx
import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Callback from './pages/Callback';
import Dashboard from './pages/Dashboard'; // Votre page existante
import ProtectedRoute from './components/ProtectedRoute';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/callback" element={<Callback />} />
        
        {/* Routes protégées */}
        <Route 
          path="/dashboard" 
          element={
            <ProtectedRoute>
              <Dashboard />
            </ProtectedRoute>
          } 
        />

        {/* Route avec permission spécifique */}
        <Route 
          path="/admin" 
          element={
            <ProtectedRoute requiredRole="AdministrateurRH">
              <AdminPage />
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

## 🔧 ÉTAPE 8 : CONFIGURATION BACKEND ASP.NET CORE

### 8.1 Installation NuGet Packages

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

### 8.2 Configuration `appsettings.json`

Ajouter dans `appsettings.json` :

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

### 8.3 Configuration `Program.cs`

Ajouter dans `Program.cs` :

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuration JWT Authentication
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

// CORS pour le frontend React
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

### 8.4 Protéger les Controllers

Ajouter `[Authorize]` sur vos controllers :

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize] // ✅ Nécessite un token JWT valide
public class EmployeesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetEmployees()
    {
        // Accéder aux claims de l'utilisateur
        var userId = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;
        var roles = User.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                        .Select(c => c.Value).ToList();

        return Ok(new { userId, email, roles });
    }

    [HttpPost]
    [Authorize(Roles = "AdministrateurRH")] // ✅ Rôle spécifique requis
    public IActionResult CreateEmployee()
    {
        return Ok();
    }
}
```

---

## 🧪 ÉTAPE 9 : TESTS

### Test 1 : Login
1. Lancer le SSO : `dotnet run --project src/ONEE.SSO.API`
2. Lancer le backend RH : `dotnet run` (port 5291)
3. Lancer le frontend RH : `npm run dev` (port 5173)
4. Aller sur http://localhost:5173
5. Cliquer sur "Se connecter avec ONEE SSO"
6. Login : `admin@onee.ma` / `Admin@123`
7. Vérifier redirection vers `/dashboard`

### Test 2 : API Calls
1. Dans le dashboard, faire un appel API vers votre backend
2. Vérifier que le token est automatiquement ajouté dans les headers
3. Vérifier que le backend accepte la requête

### Test 3 : Logout
1. Cliquer sur déconnexion
2. Vérifier la redirection vers le SSO
3. Vérifier l'impossibilité d'accéder au dashboard

---

## 📚 EXEMPLE D'UTILISATION DANS UN COMPOSANT

```jsx
import React, { useEffect, useState } from 'react';
import authService from '../auth/authService';
import apiClient from '../api/axiosConfig';

const Dashboard = () => {
  const [user, setUser] = useState(null);
  const [employees, setEmployees] = useState([]);

  useEffect(() => {
    loadUserData();
    loadEmployees();
  }, []);

  const loadUserData = async () => {
    const profile = await authService.getUserProfile();
    setUser(profile);
  };

  const loadEmployees = async () => {
    try {
      const response = await apiClient.get('/employees');
      setEmployees(response.data);
    } catch (error) {
      console.error('Erreur chargement employés:', error);
    }
  };

  const handleLogout = () => {
    authService.logout();
  };

  if (!user) return <div>Chargement...</div>;

  return (
    <div>
      <header>
        <h1>Gestion Personnel - ONEE</h1>
        <div>
          <span>👤 {user.name} ({user.email})</span>
          <button onClick={handleLogout}>🚪 Déconnexion</button>
        </div>
      </header>
      
      <main>
        <h2>Employés</h2>
        <ul>
          {employees.map(emp => (
            <li key={emp.id}>{emp.firstName} {emp.lastName}</li>
          ))}
        </ul>
      </main>
    </div>
  );
};

export default Dashboard;
```

---

## ✅ CHECKLIST FINALE

- [ ] Packages npm installés
- [ ] `authConfig.js` créé
- [ ] `authService.js` créé
- [ ] Pages Login et Callback créées
- [ ] `silent-renew.html` créé
- [ ] `ProtectedRoute` créé
- [ ] `axiosConfig.js` créé
- [ ] Routes configurées dans `App.jsx`
- [ ] Backend JWT configuré
- [ ] CORS configuré
- [ ] Controllers protégés avec `[Authorize]`
- [ ] Test login réussi ✅
- [ ] Test API calls avec token ✅
- [ ] Test logout ✅

---

## 🎯 RÉSULTAT ATTENDU

✅ L'utilisateur clique sur "Se connecter" → Redirection vers SSO
✅ Login sur le SSO → Redirection vers `/callback` → Redirection vers `/dashboard`
✅ Toutes les requêtes API incluent automatiquement le token Bearer
✅ Le backend valide le token et retourne les données
✅ Logout révoque la session SSO

---

## 🆘 SUPPORT

En cas de problème :
1. Vérifier les ports (5205 SSO, 5291 Backend RH, 5173 Frontend RH)
2. Vérifier la console navigateur pour les erreurs OIDC
3. Vérifier les logs du backend SSO et backend RH
4. Vérifier que le ClientId `gestion-personnel` existe dans la base SSO

🚀 **Votre application Gestion Personnel est maintenant intégrée avec ONEE.SSO !**
