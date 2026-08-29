# 🔐 PROMPT D'INTÉGRATION SSO - EAMS

## 📋 INFORMATIONS DE L'APPLICATION

- **Framework** : React 18.3.1 + TypeScript
- **Port Frontend** : http://localhost:5173
- **Backend** : ASP.NET Core 9
- **Port Backend** : http://localhost:5137
- **SSO Server** : http://localhost:5205
- **ClientId** : `eams-spa`
- **Scopes** : `openid profile email roles offline_access eams`
- **Custom Scopes** : `eams_user_id`, `serviceId`

---

## 🚀 ÉTAPE 1 : INSTALLATION DES DÉPENDANCES

### Frontend React + TypeScript

```bash
npm install oidc-client-ts react-router-dom
npm install -D @types/node
```

---

## 🔧 ÉTAPE 2 : CONFIGURATION OIDC (Frontend TypeScript)

Créer le fichier `src/auth/authConfig.ts` :

```typescript
import { UserManager, WebStorageStateStore, UserManagerSettings } from 'oidc-client-ts';

const AUTHORITY = 'http://localhost:5205';
const CLIENT_ID = 'eams-spa';
const CLIENT_SECRET = 'secret-eams-2024';
const REDIRECT_URI = 'http://localhost:5173/callback';
const POST_LOGOUT_REDIRECT_URI = 'http://localhost:5173';
const SILENT_REDIRECT_URI = 'http://localhost:5173/silent-renew.html';

export const oidcConfig: UserManagerSettings = {
  authority: AUTHORITY,
  client_id: CLIENT_ID,
  client_secret: CLIENT_SECRET,
  redirect_uri: REDIRECT_URI,
  post_logout_redirect_uri: POST_LOGOUT_REDIRECT_URI,
  silent_redirect_uri: SILENT_REDIRECT_URI,
  response_type: 'code',
  scope: 'openid profile email roles offline_access eams eams_user_id serviceId',
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
  console.log('✅ EAMS User loaded:', user.profile);
  console.log('📋 Custom claims:', {
    eams_user_id: user.profile.eams_user_id,
    serviceId: user.profile.serviceId
  });
});

userManager.events.addUserUnloaded(() => {
  console.log('🚪 EAMS User logged out');
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

## 🔐 ÉTAPE 3 : TYPES TYPESCRIPT

Créer le fichier `src/auth/types.ts` :

```typescript
export interface UserProfile {
  sub: string;
  email: string;
  email_verified?: boolean;
  name: string;
  given_name?: string;
  family_name?: string;
  roles?: string[];
  permissions?: string[];
  eams_user_id?: string;
  serviceId?: string;
}

export interface EamsContext {
  userId?: string;
  serviceId?: string;
}

export interface AuthState {
  isAuthenticated: boolean;
  user: UserProfile | null;
  eamsContext: EamsContext | null;
}
```

---

## 🔐 ÉTAPE 4 : SERVICE D'AUTHENTIFICATION EAMS

Créer le fichier `src/auth/authService.ts` :

```typescript
import { User } from 'oidc-client-ts';
import { userManager } from './authConfig';
import { UserProfile, EamsContext } from './types';

class AuthService {
  // Login - Redirect to SSO
  login(): Promise<void> {
    return userManager.signinRedirect();
  }

  // Handle callback from SSO
  async completeLogin(): Promise<User> {
    try {
      const user = await userManager.signinRedirectCallback();
      return user;
    } catch (error) {
      console.error('❌ Login callback error:', error);
      throw error;
    }
  }

  // Logout
  logout(): Promise<void> {
    return userManager.signoutRedirect();
  }

  // Get current user
  async getUser(): Promise<User | null> {
    return await userManager.getUser();
  }

  // Check if user is authenticated
  async isAuthenticated(): Promise<boolean> {
    const user = await this.getUser();
    return user !== null && !user.expired;
  }

  // Get access token
  async getAccessToken(): Promise<string | undefined> {
    const user = await this.getUser();
    return user?.access_token;
  }

  // Get user profile
  async getUserProfile(): Promise<UserProfile | null> {
    const user = await this.getUser();
    return user?.profile as UserProfile | null;
  }

  // Get user roles
  async getUserRoles(): Promise<string[]> {
    const user = await this.getUser();
    const profile = user?.profile as UserProfile;
    return profile?.roles || [];
  }

  // Get user permissions
  async getUserPermissions(): Promise<string[]> {
    const user = await this.getUser();
    const profile = user?.profile as UserProfile;
    return profile?.permissions || [];
  }

  // ⭐ CUSTOM: Get EAMS User ID
  async getEamsUserId(): Promise<string | undefined> {
    const user = await this.getUser();
    const profile = user?.profile as UserProfile;
    return profile?.eams_user_id;
  }

  // ⭐ CUSTOM: Get Service ID
  async getServiceId(): Promise<string | undefined> {
    const user = await this.getUser();
    const profile = user?.profile as UserProfile;
    return profile?.serviceId;
  }

  // ⭐ CUSTOM: Get all EAMS custom claims
  async getEamsContext(): Promise<EamsContext> {
    const user = await this.getUser();
    const profile = user?.profile as UserProfile;
    return {
      userId: profile?.eams_user_id,
      serviceId: profile?.serviceId
    };
  }

  // Check if user has role
  async hasRole(role: string): Promise<boolean> {
    const roles = await this.getUserRoles();
    return roles.includes(role);
  }

  // Check if user has permission
  async hasPermission(permission: string): Promise<boolean> {
    const permissions = await this.getUserPermissions();
    return permissions.includes(permission);
  }
}

export default new AuthService();
```

---

## 📄 ÉTAPE 5 : PAGES REACT TYPESCRIPT

### 5.1 Page de Login

Créer `src/pages/Login.tsx` :

```typescript
import React from 'react';
import authService from '../auth/authService';

const Login: React.FC = () => {
  const handleLogin = () => {
    authService.login();
  };

  return (
    <div style={{ 
      display: 'flex', 
      flexDirection: 'column', 
      alignItems: 'center', 
      justifyContent: 'center', 
      height: '100vh',
      backgroundColor: '#f0f2f5'
    }}>
      <h1>EAMS - ONEE</h1>
      <h2>Equipment & Asset Management System</h2>
      <p>Connectez-vous avec votre compte ONEE SSO</p>
      <button 
        onClick={handleLogin}
        style={{
          padding: '12px 24px',
          fontSize: '16px',
          backgroundColor: '#1890ff',
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

### 5.2 Page de Callback

Créer `src/pages/Callback.tsx` :

```typescript
import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import authService from '../auth/authService';

const Callback: React.FC = () => {
  const navigate = useNavigate();

  useEffect(() => {
    const completeLogin = async () => {
      try {
        const user = await authService.completeLogin();
        
        // Récupérer les custom claims EAMS
        const eamsContext = await authService.getEamsContext();
        console.log('🎯 EAMS Context:', eamsContext);
        
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
      <h2>Authentification EAMS en cours...</h2>
      <p>Veuillez patienter</p>
    </div>
  );
};

export default Callback;
```

### 5.3 Silent Renew HTML

Créer `public/silent-renew.html` :

```html
<!DOCTYPE html>
<html>
<head>
  <title>EAMS Silent Renew</title>
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

## 🛡️ ÉTAPE 6 : PROTECTED ROUTE

Créer `src/components/ProtectedRoute.tsx` :

```typescript
import React, { useEffect, useState } from 'react';
import { Navigate } from 'react-router-dom';
import authService from '../auth/authService';

interface ProtectedRouteProps {
  children: React.ReactNode;
  requiredRole?: string;
  requiredPermission?: string;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ 
  children, 
  requiredRole, 
  requiredPermission 
}) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean | null>(null);
  const [hasAccess, setHasAccess] = useState<boolean>(true);

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

  return <>{children}</>;
};

export default ProtectedRoute;
```

---

## 🔄 ÉTAPE 7 : AXIOS INTERCEPTOR

Créer `src/api/axiosConfig.ts` :

```typescript
import axios, { AxiosInstance, InternalAxiosRequestConfig, AxiosError } from 'axios';
import authService from '../auth/authService';

const API_BASE_URL = 'http://localhost:5137/api'; // Backend EAMS

const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Request interceptor - Ajouter le token + custom headers EAMS
apiClient.interceptors.request.use(
  async (config: InternalAxiosRequestConfig) => {
    const token = await authService.getAccessToken();
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    // ⭐ Ajouter les custom claims EAMS dans les headers
    const eamsContext = await authService.getEamsContext();
    if (eamsContext.userId && config.headers) {
      config.headers['X-EAMS-User-Id'] = eamsContext.userId;
    }
    if (eamsContext.serviceId && config.headers) {
      config.headers['X-EAMS-Service-Id'] = eamsContext.serviceId;
    }

    return config;
  },
  (error: AxiosError) => {
    return Promise.reject(error);
  }
);

// Response interceptor - Gérer les erreurs 401
apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
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

## 🗺️ ÉTAPE 8 : CONFIGURATION DES ROUTES

Modifier `src/App.tsx` :

```typescript
import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Callback from './pages/Callback';
import Dashboard from './pages/Dashboard'; // Votre page existante
import ProtectedRoute from './components/ProtectedRoute';

const App: React.FC = () => {
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

        {/* Route avec rôle spécifique */}
        <Route 
          path="/admin" 
          element={
            <ProtectedRoute requiredRole="ResponsableEquipements">
              <AdminPage />
            </ProtectedRoute>
          } 
        />

        <Route path="/" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </BrowserRouter>
  );
};

export default App;
```

---

## 🔧 ÉTAPE 9 : CONFIGURATION BACKEND ASP.NET CORE

### 9.1 Installation NuGet Packages

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

### 9.2 Configuration `appsettings.json`

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

### 9.3 Configuration `Program.cs`

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

### 9.4 Middleware pour Custom Claims EAMS

Créer `Middlewares/EamsContextMiddleware.cs` :

```csharp
public class EamsContextMiddleware
{
    private readonly RequestDelegate _next;

    public EamsContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Récupérer les custom claims depuis le token JWT
        var eamsUserId = context.User.FindFirst("eams_user_id")?.Value;
        var serviceId = context.User.FindFirst("serviceId")?.Value;

        // Ajouter dans HttpContext.Items pour accès dans les controllers
        context.Items["EamsUserId"] = eamsUserId;
        context.Items["ServiceId"] = serviceId;

        await _next(context);
    }
}

// Extension method
public static class EamsContextMiddlewareExtensions
{
    public static IApplicationBuilder UseEamsContext(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<EamsContextMiddleware>();
    }
}
```

Ajouter dans `Program.cs` (après `UseAuthentication`) :

```csharp
app.UseAuthentication();
app.UseEamsContext(); // ⭐ Middleware custom EAMS
app.UseAuthorization();
```

### 9.5 Protéger les Controllers avec Custom Claims

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize] // ✅ Nécessite un token JWT valide
public class EquipmentsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetEquipments()
    {
        // ⭐ Accéder aux custom claims EAMS
        var eamsUserId = HttpContext.Items["EamsUserId"]?.ToString();
        var serviceId = HttpContext.Items["ServiceId"]?.ToString();

        // Alternative: Lire directement depuis User.Claims
        var userId = User.FindFirst("eams_user_id")?.Value;
        var email = User.FindFirst("email")?.Value;
        var roles = User.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                        .Select(c => c.Value).ToList();

        return Ok(new { 
            eamsUserId, 
            serviceId,
            email, 
            roles 
        });
    }

    [HttpPost]
    [Authorize(Roles = "ResponsableEquipements")] // ✅ Rôle spécifique requis
    public IActionResult CreateEquipment([FromBody] EquipmentDto dto)
    {
        var eamsUserId = HttpContext.Items["EamsUserId"]?.ToString();
        
        // Logique métier avec eamsUserId
        
        return Ok();
    }
}
```

---

## 🧪 ÉTAPE 10 : TESTS

### Test 1 : Login
1. Lancer le SSO : `dotnet run --project src/ONEE.SSO.API`
2. Lancer le backend EAMS : `dotnet run` (port 5137)
3. Lancer le frontend EAMS : `npm run dev` (port 5173)
4. Aller sur http://localhost:5173
5. Cliquer sur "Se connecter avec ONEE SSO"
6. Login : `admin@onee.ma` / `Admin@123`
7. Vérifier redirection vers `/dashboard`
8. **Vérifier dans la console** : custom claims `eams_user_id`, `serviceId`

### Test 2 : Custom Claims dans API
1. Faire un appel API GET `/api/equipments`
2. Vérifier que les headers `X-EAMS-User-Id`, `X-EAMS-Service-Id` sont présents
3. Vérifier que le backend reçoit les claims

### Test 3 : TypeScript Compilation
1. `npm run build` → doit compiler sans erreur TypeScript

### Test 4 : Logout
1. Cliquer sur déconnexion
2. Vérifier la révocation de session

---

## 📚 EXEMPLE D'UTILISATION DANS UN COMPOSANT TYPESCRIPT

```typescript
import React, { useEffect, useState } from 'react';
import authService from '../auth/authService';
import apiClient from '../api/axiosConfig';
import { UserProfile, EamsContext } from '../auth/types';

interface Equipment {
  id: string;
  name: string;
  status: string;
}

const Dashboard: React.FC = () => {
  const [user, setUser] = useState<UserProfile | null>(null);
  const [eamsContext, setEamsContext] = useState<EamsContext | null>(null);
  const [equipments, setEquipments] = useState<Equipment[]>([]);

  useEffect(() => {
    loadUserData();
    loadEquipments();
  }, []);

  const loadUserData = async () => {
    const profile = await authService.getUserProfile();
    const context = await authService.getEamsContext();
    
    setUser(profile);
    setEamsContext(context);
    
    console.log('👤 User:', profile);
    console.log('🎯 EAMS Context:', context);
  };

  const loadEquipments = async () => {
    try {
      const response = await apiClient.get<Equipment[]>('/equipments');
      setEquipments(response.data);
    } catch (error) {
      console.error('Erreur chargement équipements:', error);
    }
  };

  const handleLogout = () => {
    authService.logout();
  };

  if (!user) return <div>Chargement...</div>;

  return (
    <div>
      <header>
        <h1>EAMS - ONEE</h1>
        <div>
          <span>👤 {user.name} ({user.email})</span>
          <span>🆔 EAMS User ID: {eamsContext?.userId}</span>
          <span>🏢 Service: {eamsContext?.serviceId}</span>
          <button onClick={handleLogout}>🚪 Déconnexion</button>
        </div>
      </header>
      
      <main>
        <h2>Équipements</h2>
        <ul>
          {equipments.map(eq => (
            <li key={eq.id}>{eq.name} - {eq.status}</li>
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

- [ ] Packages npm installés (oidc-client-ts, react-router-dom, @types/node)
- [ ] `authConfig.ts` avec scopes EAMS créé
- [ ] `types.ts` avec interfaces TypeScript créé
- [ ] `authService.ts` avec méthodes custom EAMS créé
- [ ] Pages Login et Callback créées (TypeScript)
- [ ] `silent-renew.html` créé
- [ ] `ProtectedRoute.tsx` créé
- [ ] `axiosConfig.ts` avec headers custom créé
- [ ] Routes configurées dans `App.tsx`
- [ ] Backend JWT configuré
- [ ] Middleware `EamsContext` créé et configuré
- [ ] CORS configuré
- [ ] Controllers protégés avec `[Authorize]`
- [ ] TypeScript compile sans erreur ✅
- [ ] Test login réussi ✅
- [ ] Test custom claims EAMS reçus ✅
- [ ] Test API calls avec token + headers custom ✅
- [ ] Test logout ✅

---

## 🎯 RÉSULTAT ATTENDU

✅ L'utilisateur clique sur "Se connecter" → Redirection vers SSO
✅ Login sur le SSO → Callback avec custom claims EAMS
✅ Toutes les requêtes API incluent token + headers `X-EAMS-*`
✅ Le backend extrait les claims `eams_user_id`, `serviceId`
✅ TypeScript assure la sécurité des types
✅ Logout révoque la session SSO

---

## 🆘 SUPPORT

En cas de problème :
1. Vérifier les ports (5205 SSO, 5137 Backend EAMS, 5173 Frontend EAMS)
2. Vérifier la console pour les custom claims
3. Vérifier les logs backend pour les headers `X-EAMS-*`
4. Vérifier que le ClientId `eams-spa` existe dans la base SSO
5. Vérifier la compilation TypeScript : `npm run build`

🚀 **Votre application EAMS (TypeScript) est maintenant intégrée avec ONEE.SSO + Custom Claims !**
