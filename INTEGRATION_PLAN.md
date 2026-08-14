# 🔗 Plan d'Intégration SSO avec les 3 Applications Clientes

## 🎯 Objectif

Connecter les 3 applications clientes existantes (**Gestion Personnel**, **TIMS**, **EAMS**) au microservice SSO ONEE.SSO pour permettre l'authentification unique (Single Sign-On).

---

## 📋 Applications Clientes Configurées

### 1. Gestion Personnel (RH Management)
- **ClientId** : `gestion-personnel`
- **ClientSecret** : (hashé en base de données)
- **RedirectUri** : À configurer selon votre application
- **Access Token** : 15 minutes
- **Refresh Token** : 30 jours
- **Scopes** : `openid`, `profile`, `email`, `roles`, `offline_access`

### 2. TIMS (Time Management)
- **ClientId** : `tims-app`
- **ClientSecret** : (hashé en base de données)
- **RedirectUri** : À configurer selon votre application
- **Access Token** : 60 minutes
- **Refresh Token** : 24 heures
- **Scopes** : `openid`, `profile`, `email`, `roles`, `tims_user_id`, `tims_service_id`, `tims_team_id`, `offline_access`

### 3. EAMS (Enterprise Asset Management)
- **ClientId** : `eams-spa`
- **ClientSecret** : (hashé en base de données)
- **RedirectUri** : À configurer selon votre application
- **Access Token** : 30 minutes
- **Refresh Token** : 30 jours
- **Scopes** : `openid`, `profile`, `email`, `roles`, `eams_user_id`, `serviceId`, `offline_access`

---

## 🔧 Prérequis Techniques

### Côté SSO (ONEE.SSO)
✅ **Déjà fait** :
- OIDC Discovery endpoints
- JWKS endpoint
- Token validation endpoint
- Client applications configurées en base de données
- JWT signing key configuré

### Côté Applications Clientes
**À implémenter** :
- Bibliothèque OIDC client (ex: oidc-client-ts pour Angular/React, Microsoft.AspNetCore.Authentication.OpenIdConnect pour .NET)
- Configuration de la discovery URL
- Gestion des tokens (access + refresh)
- Protection des routes
- Interception des requêtes HTTP pour ajouter le Bearer token

---

## 🚀 Étapes d'Intégration

### Phase 1 : Configuration de Base

#### Étape 1.1 : Récupérer les ClientSecrets

Les secrets sont hashés en base de données. Vous devez utiliser les secrets en clair que vous avez définis lors du seed.

**SQL pour vérifier les clients** :
```sql
SELECT ClientId, ClientName, ClientSecret, RedirectUri, AllowedScopes 
FROM ClientApplications 
WHERE IsActive = 1;
```

**⚠️ Important** : Notez les ClientSecrets **avant hachage** pour les utiliser dans vos applications.

Si vous ne les avez pas :
1. Ouvrez `src/ONEE.SSO.Infrastructure/Persistence/Seed/ClientApplicationsSeeder.cs`
2. Consultez les secrets en clair
3. OU régénérez-les et mettez à jour la base de données

---

#### Étape 1.2 : Tester OIDC Discovery

Vérifiez que le discovery endpoint fonctionne :

**URL** : `http://localhost:5205/.well-known/openid-configuration`

**Réponse attendue** :
```json
{
  "issuer": "http://localhost:5205",
  "authorization_endpoint": "http://localhost:5205/connect/authorize",
  "token_endpoint": "http://localhost:5205/api/auth/login",
  "userinfo_endpoint": "http://localhost:5205/api/auth/userinfo",
  "jwks_uri": "http://localhost:5205/.well-known/jwks.json",
  "scopes_supported": ["openid", "profile", "email", "roles", "offline_access"],
  "response_types_supported": ["code", "token", "id_token"],
  "grant_types_supported": ["authorization_code", "refresh_token"],
  "subject_types_supported": ["public"],
  "id_token_signing_alg_values_supported": ["RS256"]
}
```

---

### Phase 2 : Intégration Application par Application

## 🔷 Option A : Application Angular/React/Vue (SPA)

### Exemple pour Angular avec oidc-client-ts

#### 1. Installer la bibliothèque
```bash
npm install oidc-client-ts
```

#### 2. Créer le service d'authentification

**auth.service.ts** :
```typescript
import { Injectable } from '@angular/core';
import { UserManager, User } from 'oidc-client-ts';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private userManager: UserManager;
  private user: User | null = null;

  constructor() {
    const settings = {
      authority: 'http://localhost:5205',
      client_id: 'gestion-personnel', // ou 'tims-app' ou 'eams-spa'
      client_secret: 'VOTRE_CLIENT_SECRET',
      redirect_uri: 'http://localhost:4200/callback',
      response_type: 'code',
      scope: 'openid profile email roles offline_access',
      post_logout_redirect_uri: 'http://localhost:4200',
      automaticSilentRenew: true,
      loadUserInfo: true
    };

    this.userManager = new UserManager(settings);
  }

  async login(): Promise<void> {
    await this.userManager.signinRedirect();
  }

  async completeLogin(): Promise<User | null> {
    this.user = await this.userManager.signinRedirectCallback();
    return this.user;
  }

  async logout(): Promise<void> {
    await this.userManager.signoutRedirect();
  }

  async getAccessToken(): Promise<string | null> {
    const user = await this.userManager.getUser();
    return user?.access_token || null;
  }

  async isAuthenticated(): Promise<boolean> {
    const user = await this.userManager.getUser();
    return user !== null && !user.expired;
  }

  async getUser(): Promise<User | null> {
    return await this.userManager.getUser();
  }
}
```

#### 3. Créer le composant callback

**callback.component.ts** :
```typescript
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-callback',
  template: '<p>Processing login...</p>'
})
export class CallbackComponent implements OnInit {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  async ngOnInit(): Promise<void> {
    try {
      await this.authService.completeLogin();
      this.router.navigate(['/dashboard']);
    } catch (error) {
      console.error('Login failed:', error);
      this.router.navigate(['/login']);
    }
  }
}
```

#### 4. Créer l'intercepteur HTTP

**auth.interceptor.ts** :
```typescript
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable, from } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return from(this.authService.getAccessToken()).pipe(
      switchMap(token => {
        if (token) {
          req = req.clone({
            setHeaders: {
              Authorization: `Bearer ${token}`
            }
          });
        }
        return next.handle(req);
      })
    );
  }
}
```

#### 5. Protéger les routes

**auth.guard.ts** :
```typescript
import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  async canActivate(): Promise<boolean> {
    const isAuthenticated = await this.authService.isAuthenticated();
    
    if (!isAuthenticated) {
      await this.authService.login();
      return false;
    }
    
    return true;
  }
}
```

---

## 🔷 Option B : Application ASP.NET Core (MVC/Razor)

### 1. Installer les packages NuGet
```bash
dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

### 2. Configurer l'authentification

**Program.cs** :
```csharp
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Configuration OIDC
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = "http://localhost:5205";
    options.ClientId = "gestion-personnel"; // ou 'tims-app' ou 'eams-spa'
    options.ClientSecret = "VOTRE_CLIENT_SECRET";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("roles");
    options.Scope.Add("offline_access");
    
    options.RequireHttpsMetadata = false; // Seulement pour dev
    options.UsePkce = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

### 3. Protéger les contrôleurs

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        var username = User.Identity?.Name;
        var roles = User.Claims
            .Where(c => c.Type == "role")
            .Select(c => c.Value)
            .ToList();
        
        ViewBag.Username = username;
        ViewBag.Roles = roles;
        
        return View();
    }
}
```

### 4. Créer les actions Login/Logout

```csharp
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    public IActionResult Login(string returnUrl = "/")
    {
        return Challenge(new AuthenticationProperties 
        { 
            RedirectUri = returnUrl 
        }, OpenIdConnectDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
```

---

## 🔷 Option C : Appels API Directs (Pour Tests Rapides)

Si vous voulez tester rapidement sans bibliothèque OIDC complète :

### 1. Login Direct

**POST** `http://localhost:5205/api/auth/login`
```json
{
  "usernameOrEmail": "admin",
  "password": "Admin@123"
}
```

**Réponse** :
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "...",
  "expiresAt": "2026-08-14T12:30:00Z",
  "refreshTokenExpiresAt": "2026-09-14T11:30:00Z",
  "user": {
    "id": 1,
    "username": "admin",
    "email": "admin@onee.ma",
    "roles": ["SuperAdmin"]
  }
}
```

### 2. Appels API avec Bearer Token

**Headers** :
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 3. Refresh Token

**POST** `http://localhost:5205/api/auth/refresh`
```json
{
  "refreshToken": "VOTRE_REFRESH_TOKEN"
}
```

---

## 📊 Checklist d'Intégration

### Pour chaque application cliente :

#### Configuration
- [ ] ClientId configuré
- [ ] ClientSecret configuré
- [ ] RedirectUri configuré
- [ ] Scopes configurés
- [ ] Discovery URL testée

#### Flux d'authentification
- [ ] Bouton "Login" redirige vers SSO
- [ ] Callback traite la réponse du SSO
- [ ] Access Token stocké (sessionStorage/localStorage/cookie)
- [ ] Refresh Token stocké de manière sécurisée

#### Protection des ressources
- [ ] Intercepteur HTTP ajoute Bearer token
- [ ] Routes protégées par Guard/Middleware
- [ ] Gestion de l'expiration du token
- [ ] Refresh automatique avant expiration

#### Logout
- [ ] Bouton "Logout" révoque les tokens
- [ ] Redirection vers page de login
- [ ] Nettoyage du stockage local

#### Affichage utilisateur
- [ ] Nom d'utilisateur affiché
- [ ] Rôles affichés
- [ ] Menu adapté selon les permissions

---

## 🎯 Scénario de Démonstration SSO

### Scénario 1 : Login depuis Gestion Personnel
1. Utilisateur ouvre **Gestion Personnel** → `/login`
2. Clique sur "Se connecter"
3. Redirigé vers **ONEE.SSO** → login page
4. Entre `admin` / `Admin@123`
5. **ONEE.SSO** valide et génère JWT + Refresh Token
6. Utilisateur redirigé vers **Gestion Personnel** avec tokens
7. **Gestion Personnel** affiche le dashboard avec nom/rôles

### Scénario 2 : Accès à TIMS sans re-login
1. Utilisateur (déjà connecté) ouvre **TIMS**
2. **TIMS** vérifie le token SSO
3. Si valide → accès direct au dashboard
4. Sinon → refresh token automatique
5. Dashboard **TIMS** affiché sans demander login

### Scénario 3 : Logout global
1. Utilisateur clique "Déconnexion" dans **EAMS**
2. **EAMS** appelle `/api/auth/logout?allDevices=true`
3. **ONEE.SSO** révoque tous les tokens
4. Utilisateur redirigé vers login
5. Accès à **Gestion Personnel** ou **TIMS** → demande de login

---

## 🔒 Considérations de Sécurité

### Tokens
- ✅ Access Token : court (15-60 min)
- ✅ Refresh Token : stocké de manière sécurisée (HttpOnly cookie si possible)
- ✅ Pas de stockage de tokens en localStorage si possible (préférer sessionStorage ou cookie)

### HTTPS
- ⚠️ En production, utilisez **HTTPS obligatoire**
- ⚠️ Mettez à jour `RequireHttpsMetadata = true`

### CORS
- ⚠️ Configurez CORS sur ONEE.SSO pour autoriser vos applications clientes
- Ajoutez les origines dans `appsettings.json`

### RedirectUri
- ✅ Validez strictement les RedirectUri en base de données
- ⚠️ Pas de wildcards en production

---

## 📝 Prochaines Étapes

### Étape 1 : Tester manuellement avec Swagger ✅
**Fait** : Guide de tests créé (`TESTING_GUIDE_SPRINT3.md`)

### Étape 2 : Choisir une application cliente pour l'intégration
Laquelle commencer ?
- **Gestion Personnel** (RH)
- **TIMS** (Temps)
- **EAMS** (Actifs)

### Étape 3 : Implémenter l'authentification côté client
Selon la technologie de votre application :
- Angular/React/Vue → Option A
- ASP.NET Core → Option B
- API directe → Option C

### Étape 4 : Tester le flow SSO complet
- Login
- Refresh
- Logout
- Multi-application

### Étape 5 : Documenter et préparer la démonstration
- Captures d'écran
- Vidéo de démonstration
- Documentation utilisateur

---

## 🎓 Pour la Soutenance

### Démontrer :
1. ✅ Architecture SSO centralisée
2. ✅ 3 applications clientes configurées
3. ✅ Login unique → accès aux 3 applications
4. ✅ Logout global → révocation partout
5. ✅ Sécurité (blocage, audit, tokens)
6. ✅ OIDC standard (discovery, JWKS)

### Préparer :
- [ ] Diagramme d'architecture
- [ ] Flow sequence diagram (login/refresh/logout)
- [ ] Démonstration live
- [ ] Code review des points clés
- [ ] Explication des choix techniques

---

**Bon courage pour l'intégration ! 🚀**
