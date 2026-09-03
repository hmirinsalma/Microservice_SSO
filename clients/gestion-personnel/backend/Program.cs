using System.Text;
using FluentValidation;
using GestionPersonnel.API.Data;
using GestionPersonnel.API.Exceptions;
using GestionPersonnel.API.Mappings;
using GestionPersonnel.API.Repositories;
using GestionPersonnel.API.Repositories.Interfaces;
using GestionPersonnel.API.Services;
using GestionPersonnel.API.Services.Interfaces;
using GestionPersonnel.API.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ─── DbContext ───────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─── AutoMapper ──────────────────────────────────────────────
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ─── Repositories ────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDirectionRepository, DirectionRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IEmployeRepository, EmployeRepository>();
builder.Services.AddScoped<IUserManagementRepository, UserManagementRepository>();

// ─── Services ────────────────────────────────────────────────
// SSO-READY : Changer uniquement cette ligne lors de l'intégration SSO :
//   builder.Services.AddScoped<IAuthService, SsoAuthService>();
builder.Services.AddScoped<IAuthService, StubAuthService>();

// 🎯 NOUVEAU: Service de provisioning automatique SSO
builder.Services.AddScoped<SsoProvisioningService>();

// 🔧 HttpClient pour appeler le SSO depuis AuthController
builder.Services.AddHttpClient();

// TEMPORAIRE — Supprimé lors de l'intégration SSO :
builder.Services.AddScoped<IStubCredentialService, StubCredentialService>();
builder.Services.AddScoped<IDirectionService, DirectionService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IEmployeService, EmployeService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<ICongeService, CongeService>();

// ─── Validators ──────────────────────────────────────────────
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();

// ─── Exception Handler ───────────────────────────────────────
builder.Services.AddTransient<GlobalExceptionHandler>();

// ─── JWT — Validation du token SSO ─────
var jwtSecret = builder.Configuration["Jwt:Secret"]!;
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
{
    KeyId = "onee-sso-key-2024" // IMPORTANT: Doit correspondre au kid du token JWT généré par le SSO
};

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // SSO Configuration - valide les tokens JWT émis par ONEE.SSO
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = signingKey,
            ValidateIssuer           = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"], // ONEE.SSO
            ValidateAudience         = true,
            ValidAudience            = builder.Configuration["Jwt:Audience"], // ONEE.Applications
            ValidateLifetime         = true,
            ClockSkew                = TimeSpan.Zero,
        };
        
        // Événements pour debug SSO
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"❌ SSO Auth Failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var userId = context.Principal?.FindFirst("sub")?.Value;
                var email = context.Principal?.FindFirst("email")?.Value;
                Console.WriteLine($"✅ SSO Token Validated - User: {email} (ID: {userId})");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// ─── CORS ────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            builder.Configuration["Cors:AllowedOrigins"]?.Split(',') ?? ["http://localhost:5174"]
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// ─── Controllers ─────────────────────────────────────────────
builder.Services.AddControllers();

// ─── Swagger ─────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gestion du Personnel API",
        Version = "v1",
        Description = "API de gestion du personnel - ONEE"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez votre token JWT: Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ─── Middleware Pipeline ──────────────────────────────────────
app.UseMiddleware<GlobalExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestion Personnel v1"));
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ─── Auto-migrate + Seed complet ─────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // ═══════════════════════════════════════════════════════════
    // SEED UTILISATEURS + EMPLOYÉS LIÉS
    // ═══════════════════════════════════════════════════════════

    // Rôles : 1=AdminRH  2=Directeur  3=ChefDeService  4=Employe
    // Directions : 1=RH  2=Technique  3=Informatique  4=Patrimoine
    // Services :
    //   1=Recrutement(RH)  2=Formation(RH)  3=Paie(RH)
    //   4=Maintenance(Tech) 5=Exploitation(Tech)
    //   6=Développement(Info) 7=Infrastructure(Info)
    //   8=Immobilier(Patri)  9=Logistique(Patri)

    var seedData = new[]
    {
        // ── Admin RH ──────────────────────────────────────────
        new { Username="admin",        Email="admin@onee.ma",          Pwd="Admin@123",      RoleId=1,
              Nom="Admin",      Prenom="RH",        Matricule="ADM-001", Poste="Responsable RH",          DirId=1, SvcId=1, EmpId=(int?)null },

        // ── Directeurs (1 par direction) ──────────────────────
        new { Username="dir.rh",       Email="dir.rh@onee.ma",         Pwd="DirRH@123",      RoleId=2,
              Nom="Benali",     Prenom="Mohamed",   Matricule="DIR-001", Poste="Directeur",               DirId=1, SvcId=1, EmpId=(int?)null },

        new { Username="dir.tech",     Email="dir.tech@onee.ma",       Pwd="DirTech@123",    RoleId=2,
              Nom="Alaoui",     Prenom="Rachid",    Matricule="DIR-002", Poste="Directeur",               DirId=2, SvcId=4, EmpId=(int?)null },

        new { Username="dir.info",     Email="dir.info@onee.ma",       Pwd="DirInfo@123",    RoleId=2,
              Nom="Chraibi",    Prenom="Khalid",    Matricule="DIR-003", Poste="Directeur",               DirId=3, SvcId=6, EmpId=(int?)null },

        new { Username="dir.patri",    Email="dir.patri@onee.ma",      Pwd="DirPatri@123",   RoleId=2,
              Nom="Tazi",       Prenom="Hassan",    Matricule="DIR-004", Poste="Directeur",               DirId=4, SvcId=8, EmpId=(int?)null },

        // ── Chefs de service (1 par service) ──────────────────
        new { Username="chef.recruit",  Email="chef.recrutement@onee.ma", Pwd="Chef@123",    RoleId=3,
              Nom="Fassi",      Prenom="Nadia",     Matricule="CHF-001", Poste="Chef de Service",         DirId=1, SvcId=1, EmpId=(int?)null },

        new { Username="chef.forma",   Email="chef.formation@onee.ma",  Pwd="Chef@123",     RoleId=3,
              Nom="Berrada",    Prenom="Samira",    Matricule="CHF-002", Poste="Chef de Service",         DirId=1, SvcId=2, EmpId=(int?)null },

        new { Username="chef.paie",    Email="chef.paie@onee.ma",       Pwd="Chef@123",     RoleId=3,
              Nom="Ouhbi",      Prenom="Youssef",   Matricule="CHF-003", Poste="Chef de Service",         DirId=1, SvcId=3, EmpId=(int?)null },

        new { Username="chef.maint",   Email="chef.maintenance@onee.ma",Pwd="Chef@123",     RoleId=3,
              Nom="Benhaddou",  Prenom="Omar",      Matricule="CHF-004", Poste="Chef de Service",         DirId=2, SvcId=4, EmpId=(int?)null },

        new { Username="chef.exploit", Email="chef.exploit@onee.ma",    Pwd="Chef@123",     RoleId=3,
              Nom="Lahlou",     Prenom="Fatima",    Matricule="CHF-005", Poste="Chef de Service",         DirId=2, SvcId=5, EmpId=(int?)null },

        new { Username="chef.dev",     Email="chef.dev@onee.ma",        Pwd="Chef@123",     RoleId=3,
              Nom="Hmiri",      Prenom="Salma",     Matricule="CHF-006", Poste="Chef de Service",         DirId=3, SvcId=6, EmpId=(int?)null },

        new { Username="chef.infra",   Email="chef.infra@onee.ma",      Pwd="Chef@123",     RoleId=3,
              Nom="Sekkat",     Prenom="Amine",     Matricule="CHF-007", Poste="Chef de Service",         DirId=3, SvcId=7, EmpId=(int?)null },

        new { Username="chef.immo",    Email="chef.immobilier@onee.ma", Pwd="Chef@123",     RoleId=3,
              Nom="Cherkaoui",  Prenom="Leila",     Matricule="CHF-008", Poste="Chef de Service",         DirId=4, SvcId=8, EmpId=(int?)null },

        new { Username="chef.logis",   Email="chef.logistique@onee.ma", Pwd="Chef@123",     RoleId=3,
              Nom="Moussaoui",  Prenom="Karim",     Matricule="CHF-009", Poste="Chef de Service",         DirId=4, SvcId=9, EmpId=(int?)null },

        // ── Employés (2 par service) ───────────────────────────
        new { Username="emp.recruit1", Email="a.benaissa@onee.ma",      Pwd="Emp@123",      RoleId=4,
              Nom="Benaissa",   Prenom="Aicha",     Matricule="EMP-001", Poste="Chargé de Recrutement",   DirId=1, SvcId=1, EmpId=(int?)null },

        new { Username="emp.recruit2", Email="m.karimi@onee.ma",        Pwd="Emp@123",      RoleId=4,
              Nom="Karimi",     Prenom="Mohammed",  Matricule="EMP-002", Poste="Chargé de Recrutement",   DirId=1, SvcId=1, EmpId=(int?)null },

        new { Username="emp.forma1",   Email="s.idrissi@onee.ma",       Pwd="Emp@123",      RoleId=4,
              Nom="Idrissi",    Prenom="Sara",      Matricule="EMP-003", Poste="Chargé de Formation",     DirId=1, SvcId=2, EmpId=(int?)null },

        new { Username="emp.forma2",   Email="r.benomar@onee.ma",       Pwd="Emp@123",      RoleId=4,
              Nom="Benomar",    Prenom="Reda",      Matricule="EMP-004", Poste="Chargé de Formation",     DirId=1, SvcId=2, EmpId=(int?)null },

        new { Username="emp.paie1",    Email="h.tahiri@onee.ma",        Pwd="Emp@123",      RoleId=4,
              Nom="Tahiri",     Prenom="Hind",      Matricule="EMP-005", Poste="Gestionnaire de Paie",    DirId=1, SvcId=3, EmpId=(int?)null },

        new { Username="emp.paie2",    Email="y.amrani@onee.ma",        Pwd="Emp@123",      RoleId=4,
              Nom="Amrani",     Prenom="Yassine",   Matricule="EMP-006", Poste="Gestionnaire de Paie",    DirId=1, SvcId=3, EmpId=(int?)null },

        new { Username="emp.maint1",   Email="n.benkirane@onee.ma",     Pwd="Emp@123",      RoleId=4,
              Nom="Benkirane",  Prenom="Nour",      Matricule="EMP-007", Poste="Technicien Maintenance",  DirId=2, SvcId=4, EmpId=(int?)null },

        new { Username="emp.maint2",   Email="o.slaoui@onee.ma",        Pwd="Emp@123",      RoleId=4,
              Nom="Slaoui",     Prenom="Omar",      Matricule="EMP-008", Poste="Technicien Maintenance",  DirId=2, SvcId=4, EmpId=(int?)null },

        new { Username="emp.exploit1", Email="f.ziani@onee.ma",         Pwd="Emp@123",      RoleId=4,
              Nom="Ziani",      Prenom="Fatima",    Matricule="EMP-009", Poste="Ingénieur Exploitation",  DirId=2, SvcId=5, EmpId=(int?)null },

        new { Username="emp.exploit2", Email="a.regragui@onee.ma",      Pwd="Emp@123",      RoleId=4,
              Nom="Regragui",   Prenom="Adil",      Matricule="EMP-010", Poste="Ingénieur Exploitation",  DirId=2, SvcId=5, EmpId=(int?)null },

        new { Username="emp.dev1",     Email="i.bensouda@onee.ma",      Pwd="Emp@123",      RoleId=4,
              Nom="Bensouda",   Prenom="Imane",     Matricule="EMP-011", Poste="Développeur Full Stack",  DirId=3, SvcId=6, EmpId=(int?)null },

        new { Username="emp.dev2",     Email="k.filali@onee.ma",        Pwd="Emp@123",      RoleId=4,
              Nom="Filali",     Prenom="Khalid",    Matricule="EMP-012", Poste="Développeur Backend",     DirId=3, SvcId=6, EmpId=(int?)null },

        new { Username="emp.infra1",   Email="l.bennani@onee.ma",       Pwd="Emp@123",      RoleId=4,
              Nom="Bennani",    Prenom="Loubna",    Matricule="EMP-013", Poste="Administrateur Réseau",   DirId=3, SvcId=7, EmpId=(int?)null },

        new { Username="emp.infra2",   Email="m.ouazzani@onee.ma",      Pwd="Emp@123",      RoleId=4,
              Nom="Ouazzani",   Prenom="Mehdi",     Matricule="EMP-014", Poste="Administrateur Réseau",   DirId=3, SvcId=7, EmpId=(int?)null },

        new { Username="emp.immo1",    Email="z.benali@onee.ma",        Pwd="Emp@123",      RoleId=4,
              Nom="Benali",     Prenom="Zineb",     Matricule="EMP-015", Poste="Gestionnaire Immobilier", DirId=4, SvcId=8, EmpId=(int?)null },

        new { Username="emp.immo2",    Email="r.laroui@onee.ma",        Pwd="Emp@123",      RoleId=4,
              Nom="Laroui",     Prenom="Rachid",    Matricule="EMP-016", Poste="Gestionnaire Immobilier", DirId=4, SvcId=8, EmpId=(int?)null },

        new { Username="emp.logis1",   Email="s.belyazid@onee.ma",      Pwd="Emp@123",      RoleId=4,
              Nom="Belyazid",   Prenom="Soukaina",  Matricule="EMP-017", Poste="Responsable Logistique",  DirId=4, SvcId=9, EmpId=(int?)null },

        new { Username="emp.logis2",   Email="a.mansouri@onee.ma",      Pwd="Emp@123",      RoleId=4,
              Nom="Mansouri",   Prenom="Anas",      Matricule="EMP-018", Poste="Responsable Logistique",  DirId=4, SvcId=9, EmpId=(int?)null },
    };

    var baseDate = new DateTime(2020, 1, 1);

    foreach (var s in seedData)
    {
        // 1. Créer ou vérifier le compte User (sans PasswordHash)
        var userExisting = db.Users.FirstOrDefault(u => u.Email == s.Email);
        if (userExisting == null)
        {
            userExisting = new GestionPersonnel.API.Models.User
            {
                Username  = s.Username,
                Email     = s.Email,
                RoleId    = s.RoleId,
                IsActive  = true,
                CreatedAt = DateTime.UtcNow,
            };
            db.Users.Add(userExisting);
            db.SaveChanges();
            Console.WriteLine($">>> User créé : {s.Email}");
        }

        // 1b. Créer ou vérifier le credential stub (temporaire)
        if (!db.StubCredentials.Any(c => c.UserId == userExisting.Id))
        {
            db.StubCredentials.Add(new GestionPersonnel.API.Models.StubCredential
            {
                UserId       = userExisting.Id,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(s.Pwd),
            });
            db.SaveChanges();
            Console.WriteLine($">>> StubCredential créé pour : {s.Email} / {s.Pwd}");
        }

        // 2. Créer ou vérifier la fiche Employé (sauf Admin)
        if (s.RoleId != 1)
        {
            var empExisting = db.Employes.FirstOrDefault(e => e.Matricule == s.Matricule);
            if (empExisting == null)
            {
                var emp = new GestionPersonnel.API.Models.Employe
                {
                    Matricule    = s.Matricule,
                    Nom          = s.Nom,
                    Prenom       = s.Prenom,
                    Email        = s.Email,
                    Poste        = s.Poste,
                    DirectionId  = s.DirId,
                    ServiceId    = s.SvcId,
                    Statut       = GestionPersonnel.API.Models.StatutEmploye.Actif,
                    DateEmbauche = baseDate.AddDays(new Random().Next(0, 1000)),
                    CreatedAt    = DateTime.UtcNow,
                    UserId       = userExisting.Id, // Lien direct
                };
                db.Employes.Add(emp);
                db.SaveChanges();
                Console.WriteLine($">>> Employé créé et lié : {s.Matricule} → User {s.Email}");
            }
            else if (empExisting.UserId == null)
            {
                // Lier si pas encore lié
                empExisting.UserId = userExisting.Id;
                db.SaveChanges();
                Console.WriteLine($">>> Lien ajouté : {s.Matricule} → {s.Email}");
            }
        }
    }

    Console.WriteLine(">>> Seed complet terminé.");

    // ═══════════════════════════════════════════════════════════
    // SEED RESPONSABLES HIÉRARCHIQUES
    // Règle :
    //   Employé       → responsable = Chef du même service
    //   Chef service  → responsable = Directeur de la même direction
    //   Directeur     → responsable = Admin RH (ADM-001)
    // ═══════════════════════════════════════════════════════════

    var adminEmp = db.Employes.FirstOrDefault(e => e.Matricule == "ADM-001");

    var allEmployes = db.Employes
        .Include(e => e.User).ThenInclude(u => u!.Role)
        .ToList();

    bool changed = false;

    foreach (var emp in allEmployes)
    {
        if (emp.Matricule == "ADM-001") continue; // L'admin n'a pas de responsable

        int? newRespo = null;
        var role = emp.User?.Role?.Nom;

        if (role == "Directeur")
        {
            // Responsable du Directeur = Admin RH
            newRespo = adminEmp?.Id;
        }
        else if (role == "ChefDeService")
        {
            // Responsable du Chef = Directeur de sa direction
            var directeur = allEmployes.FirstOrDefault(e =>
                e.DirectionId == emp.DirectionId &&
                e.User?.Role?.Nom == "Directeur");
            newRespo = directeur?.Id;
        }
        else if (role == "Employe")
        {
            // Responsable de l'Employé = Chef du même service
            var chef = allEmployes.FirstOrDefault(e =>
                e.ServiceId == emp.ServiceId &&
                e.User?.Role?.Nom == "ChefDeService");
            newRespo = chef?.Id;
        }

        if (newRespo.HasValue && emp.ResponsableId != newRespo)
        {
            emp.ResponsableId = newRespo;
            changed = true;
            Console.WriteLine($">>> Responsable assigné : {emp.Matricule} ({emp.Nom}) → {allEmployes.First(e => e.Id == newRespo).Nom}");
        }
    }

    if (changed)
    {
        db.SaveChanges();
        Console.WriteLine(">>> Responsables mis à jour.");
    }
}

app.Run();
