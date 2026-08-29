using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using TIMS.API.Data;
using TIMS.API.Interfaces;
using TIMS.API.Middleware;
using TIMS.API.Services;

// ══════════════════════════════════════════════════════════════════════════════
// TIMS API — SSO-Ready Architecture
//
// AUTHENTIFICATION ACTUELLE : StubAuthService (JWT local temporaire)
//
// MIGRATION SSO — Uniquement ces 3 lignes à modifier :
//   1. Remplacer : services.AddScoped<IAuthService, StubAuthService>()
//      Par       : services.AddScoped<IAuthService, SsoAuthService>()
//   2. Remplacer : IssuerSigningKey = new SymmetricSecurityKey(...)
//      Par       : IssuerSigningKey = new RsaSecurityKey(ssoPublicKey)
//   3. Adapter Issuer/Audience selon la configuration SSO
//
// Aucun Controller métier, Service métier, Repository ou Entity métier
// ne sera modifié lors de cette migration.
// ══════════════════════════════════════════════════════════════════════════════

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/tims-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// ── Database ──────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── JWT Authentication ─────────────────────────────────────────────────────────
// ⚠️ STUB : Clé symétrique locale temporaire.
// SSO Migration : Remplacer par la clé publique RSA du microservice SSO.
var jwtKey = builder.Configuration["Jwt:Key"]!;
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
{
    KeyId = "onee-sso-key-2024" // IMPORTANT: Doit correspondre au kid du token JWT généré par le SSO
};

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            // ── STUB : clé symétrique locale ──────────────────────────────────
            // SSO Migration : new RsaSecurityKey(RsaKeyFromSso)
            IssuerSigningKey = signingKey,
            // ─────────────────────────────────────────────────────────────────
            ValidateIssuer   = true,
            ValidIssuer      = builder.Configuration["Jwt:Issuer"],   // SSO Migration : URI du SSO
            ValidateAudience = true,
            ValidAudience    = builder.Configuration["Jwt:Audience"], // SSO Migration : client_id
            ValidateLifetime = true,
            ClockSkew        = TimeSpan.Zero,
            // Claims mapping SSO-compatible
            NameClaimType = "tims_user_id",
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        };
    });

builder.Services.AddAuthorization();

// ── CORS ───────────────────────────────────────────────────────────────────────
var origins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(opt => opt.AddPolicy("TIMSPolicy", p =>
    p.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

// ── Rate Limiting ──────────────────────────────────────────────────────────────
builder.Services.AddRateLimiter(opt =>
{
    opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
        RateLimitPartition.GetFixedWindowLimiter(
            ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100, Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst, QueueLimit = 0
            }));
    opt.RejectionStatusCode = 429;
});

// ── Dependency Injection ───────────────────────────────────────────────────────
//
// ⚠️ STUB TEMPORAIRE — Remplacer par SsoAuthService lors de l'intégration SSO
builder.Services.AddScoped<IAuthService,          StubAuthService>();
builder.Services.AddScoped<IStubPasswordService,  StubPasswordService>();
// SSO Migration :
// builder.Services.AddScoped<IAuthService, SsoAuthService>();
// Supprimer : builder.Services.AddScoped<IStubPasswordService, StubPasswordService>();
//
// Services métier — Ne jamais modifier lors de la migration SSO
builder.Services.AddScoped<IInterventionService,  InterventionService>();
builder.Services.AddScoped<IUserService,          UserService>();
builder.Services.AddScoped<IDashboardService,     DashboardService>();
builder.Services.AddScoped<INotificationService,  NotificationService>();
builder.Services.AddScoped<IServiceEquipeService, ServiceEquipeService>();

// ── AutoMapper ─────────────────────────────────────────────────────────────────
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ── Controllers ────────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Swagger ────────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "ONEE TIMS API",
        Version     = "v1",
        Description = "Technical Intervention Management System — SSO-Ready Architecture\n\n" +
                      "⚠️ Mode actuel : StubAuthService (JWT local temporaire)\n" +
                      "🔜 Migration SSO : Remplacer StubAuthService → SsoAuthService dans Program.cs"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", Type = SecuritySchemeType.Http,
        Scheme = "Bearer", BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT généré par StubAuthService (temporaire). Format : Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {{
        new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
        Array.Empty<string>()
    }});
});

// ── Health Checks ──────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ── Database Migration + Seed ──────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        db.Database.Migrate();
        await DbSeeder.SeedAsync(db);
    }
    catch (Exception ex)
    {
        Log.Warning($"Database initialization warning: {ex.Message}. The application will continue but database may need manual setup.");
    }
}

// ── Middleware Pipeline ────────────────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TIMS API v1"));
}

app.UseHttpsRedirection();

app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["X-Frame-Options"]        = "DENY";
    ctx.Response.Headers["Content-Security-Policy"] = "default-src 'self'";
    await next();
});

app.UseStaticFiles();
app.UseCors("TIMSPolicy");
app.UseRateLimiter();
app.UseAuthentication();
app.UseTimsContext(); // ⭐ Middleware custom TIMS pour extraire les custom claims
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
