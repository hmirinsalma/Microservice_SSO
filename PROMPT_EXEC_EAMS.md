# 🚀 PROMPT D'EXÉCUTION - INTÉGRATION SSO EAMS

## CONTEXTE DU PROJET

Je développe **ONEE.SSO**, un serveur SSO complet en ASP.NET Core 9 qui fonctionne déjà (tests backend 100% réussis). Le serveur SSO tourne sur **http://localhost:5205**.

J'ai 3 applications clientes existantes à intégrer. Tu vas m'aider à intégrer la troisième : **EAMS (Equipment & Asset Management System)**.

---

## INFORMATIONS DE L'APPLICATION EAMS

- **Framework Frontend** : React 18.3.1 + TypeScript
- **Port Frontend** : http://localhost:5173
- **Backend** : ASP.NET Core 9
- **Port Backend** : http://localhost:5137
- **Dossier Projet Frontend** : `c:\Users\XPS\Desktop\eams-frontend`
- **Dossier Projet Backend** : `c:\Users\XPS\Desktop\eams-backend`
- **Custom Scopes** : `eams_user_id`, `serviceId`

---

## INFORMATIONS DU SERVEUR SSO

- **URL SSO** : http://localhost:5205
- **ClientId** : `eams-spa`
- **ClientSecret** : `secret-eams-2024`
- **Scopes** : `openid profile email roles offline_access eams eams_user_id serviceId`
- **Endpoints OIDC** :
  - Authorization : `http://localhost:5205/connect/authorize`
  - Token : `http://localhost:5205/connect/token`
  - Userinfo : `http://localhost:5205/api/auth/userinfo`
  - Logout : `http://localhost:5205/connect/logout`
  - Discovery : `http://localhost:5205/.well-known/openid-configuration`

---

## 🎯 TON TRAVAIL : INTÉGRER LE SSO DANS EAMS (TYPESCRIPT) AVEC CUSTOM SCOPES

### ÉTAPE 1 : INSTALLATION DES PACKAGES NPM (Frontend TypeScript)

Dans le dossier frontend, exécute :
```bash
cd c:\Users\XPS\Desktop\eams-frontend
npm install oidc-client-ts react-router-dom
npm install -D @types/node
```

---

### ÉTAPE 2 : CRÉER LES TYPES TYPESCRIPT

Crée le fichier `src/auth/types.ts` avec ce contenu exact :

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

### ÉTAPE 3 : CRÉER LA CONFIGURATION OIDC TYPESCRIPT

Crée le fichier `src/auth/authConfig.ts` avec ce contenu exact :

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

### ÉTAPE 4 : CRÉER LE SERVICE D'AUTHENTIFICATION TYPESCRIPT

Crée le fichier `src/auth/authService.ts` avec ce contenu exact :

```typescript
import { User } from 'oidc-client-ts';
import { userManager } from './authConfig';
import { UserProfile, EamsContext } from './types';

class AuthService {
  login(): Promise<void> {
    return userManager.signinRedirect();
  }

  async completeLogin(): Promise<User> {
    try {
      const user = await userManager.signinRedirectCallback();
      return user;
    } catch (error) {
      console.error('❌ Login callback error:', error);
      throw error;
    }
  }

  logout(): Promise<void> {
    return userManager.signoutRedirect();
  }

  async getUser(): Promise<User | null> {
    return await userManager.getUser();
  }

  async isAuthenticated(): Promise<boolean> {
    const user = await this.getUser();
    return user !== null && !user.expired;
  }

  async getAccessToken(): Promise<string | undefined> {
    const user = await this.getUser();
    return user?.access_token;
  }

  async getUserProfile(): Promise<UserProfile | null> {
    const user = await this.getUser();
    return user?.profile as UserProfile | null;
  }

  async getUserRoles(): Promise<string[]> {
    const user = await this.getUser();
    const profile = user?.profile as UserProfile;
    return profile?.roles || [];
  }

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

  async hasRole(role: string): Promise<boolean> {
    const roles = await this.getUserRoles();
    return roles.includes(role);
  }

  async hasPermission(permission: string): Promise<boolean> {
    const permissions = await this.getUserPermissions();
    return permissions.includes(permission);
  }
}

export default new AuthService();
```

---

### ÉTAPE 5 : CRÉER LA PAGE DE LOGIN

Crée le fichier `src/pages/Login.tsx` :

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

---

### ÉTAPE 6 : CRÉER LA PAGE DE CALLBACK

Crée le fichier `src/pages/Callback.tsx` :

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
      <h2>Authentification EAMS en cours...</h2>
      <p>Veuillez patienter</p>
    </div>
  );
};

export default Callback;
```

---

### ÉTAPE 7 : CRÉER LE SILENT RENEW HTML

Crée le fichier `public/silent-renew.html` :

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

### ÉTAPE 8 : CRÉER LE PROTECTED ROUTE

Crée le fichier `src/components/ProtectedRoute.tsx` :

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

  return <>{children}</>;
};

export default ProtectedRoute;
```

---

### ÉTAPE 9 : CRÉER L'AXIOS INTERCEPTOR TYPESCRIPT

Crée le fichier `src/api/axiosConfig.ts` :

```typescript
import axios, { AxiosInstance, InternalAxiosRequestConfig, AxiosError } from 'axios';
import authService from '../auth/authService';

const API_BASE_URL = 'http://localhost:5137/api';

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

// Response interceptor
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

### ÉTAPE 10 : MODIFIER APP.TSX

Modifie le fichier `src/App.tsx` pour ajouter les routes :

```typescript
import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Callback from './pages/Callback';
import Dashboard from './pages/Dashboard'; // Ta page existante
import ProtectedRoute from './components/ProtectedRoute';

const App: React.FC = () => {
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

### ÉTAPE 11 : CONFIGURATION BACKEND ASP.NET CORE

#### 11.1 Installation Package
Dans le backend, exécute :
```bash
cd c:\Users\XPS\Desktop\eams-backend
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

#### 11.2 Modifier appsettings.json
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

#### 11.3 Créer le Middleware EAMS Context

Crée le fichier `Middlewares/EamsContextMiddleware.cs` :

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

        // Ajouter dans HttpContext.Items
        context.Items["EamsUserId"] = eamsUserId;
        context.Items["ServiceId"] = serviceId;

        await _next(context);
    }
}

public static class EamsContextMiddlewareExtensions
{
    public static IApplicationBuilder UseEamsContext(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<EamsContextMiddleware>();
    }
}
```

#### 11.4 Modifier Program.cs
Modifie `Program.cs` :

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
app.UseEamsContext(); // ⭐ Middleware custom EAMS
app.UseAuthorization();
app.MapControllers();

app.Run();
```

#### 11.5 Exemple de Controller avec Custom Claims

Crée ou modifie un controller, par exemple `Controllers/EquipmentsController.cs` :

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquipmentsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetEquipments()
    {
        // ⭐ Accéder aux custom claims EAMS
        var eamsUserId = HttpContext.Items["EamsUserId"]?.ToString();
        var serviceId = HttpContext.Items["ServiceId"]?.ToString();

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
    [Authorize(Roles = "ResponsableEquipements")]
    public IActionResult CreateEquipment([FromBody] EquipmentDto dto)
    {
        var eamsUserId = HttpContext.Items["EamsUserId"]?.ToString();
        
        // Logique métier avec eamsUserId
        
        return Ok();
    }
}
```

---

## 🧪 ÉTAPE 12 : TESTS

### Test 1 : Lancer les 3 serveurs
1. SSO : `dotnet run --project c:\Users\XPS\source\repos\ONEE.SSO\src\ONEE.SSO.API`
2. Backend EAMS : `cd c:\Users\XPS\Desktop\eams-backend && dotnet run`
3. Frontend EAMS : `cd c:\Users\XPS\Desktop\eams-frontend && npm run dev`

### Test 2 : Login avec custom claims TypeScript
1. Aller sur http://localhost:5173
2. Cliquer "Se connecter avec ONEE SSO"
3. Login : `admin@onee.ma` / `Admin@123`
4. **Vérifier dans la console** : custom claims `eams_user_id`, `serviceId`
5. Vérifier redirection vers `/dashboard`

### Test 3 : Compilation TypeScript
1. Exécuter `npm run build` dans le frontend
2. Vérifier qu'il n'y a pas d'erreurs TypeScript

### Test 4 : API Call avec custom headers
1. Dans Dashboard, faire un appel API GET `/api/equipments`
2. Ouvrir DevTools → Network → Vérifier les headers
3. Vérifier que `X-EAMS-User-Id`, `X-EAMS-Service-Id` sont présents
4. Vérifier que le backend retourne les custom claims

### Test 5 : Logout
1. Cliquer sur déconnexion
2. Vérifier la redirection

---

## ✅ RÉSULTAT ATTENDU

✅ Login → Redirection SSO → Callback avec custom claims EAMS  
✅ Console affiche `eams_user_id`, `serviceId`  
✅ TypeScript compile sans erreur  
✅ Toutes les requêtes API incluent token + headers `X-EAMS-*`  
✅ Backend extrait les custom claims du JWT  
✅ Logout révoque la session  

---

## 🎯 INSTRUCTIONS POUR TOI (KIRO)

**EXÉCUTE TOUTES CES ÉTAPES DANS L'ORDRE :**

1. ✅ Installe les packages npm (y compris @types/node)
2. ✅ Crée le fichier `types.ts` avec les interfaces TypeScript
3. ✅ Crée tous les fichiers TypeScript (.ts/.tsx) avec custom scopes EAMS
4. ✅ Installe le package NuGet dans le backend
5. ✅ Crée le middleware `EamsContextMiddleware.cs`
6. ✅ Modifie `appsettings.json` et `Program.cs` du backend
7. ✅ Crée un exemple de controller avec custom claims
8. ✅ Valide que tous les fichiers sont créés
9. ✅ Affiche un résumé de ce qui a été fait

**NE POSE PAS DE QUESTIONS, EXÉCUTE DIRECTEMENT !**
