using ONEE.SSO.API.Extensions;
using ONEE.SSO.Application.DependencyInjection;
using ONEE.SSO.Infrastructure.DependencyInjection;
using Serilog;
using Microsoft.EntityFrameworkCore;
using ONEE.SSO.Infrastructure.Persistence;
using ONEE.SSO.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

Console.WriteLine("== D�but Program ==");


var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("== Builder cr�� ==");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSection = builder.Configuration.GetSection("Jwt");

        var secretKey = jwtSection["SecretKey"]
            ?? throw new InvalidOperationException(
                "JWT SecretKey is not configured.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey)),

            ClockSkew = TimeSpan.Zero
        };
    });

// Ajouter Razor Pages
builder.Services.AddRazorPages();

// Ajouter HttpClientFactory pour les appels API
builder.Services.AddHttpClient();

// Ajouter IHttpContextAccessor pour accéder au HttpContext dans les contrôleurs
builder.Services.AddHttpContextAccessor();

// Ajouter le store de codes d'autorisation (singleton partagé)
builder.Services.AddSingleton<ONEE.SSO.API.Services.AuthorizationCodeStore>();

// Configurer CORS pour autoriser les applications clientes
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClients", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",  // RH Frontend
            "http://localhost:5174",  // RH Frontend (ancien port)
            "http://localhost:5175",  // TIMS Frontend
            "http://localhost:5291",  // RH Backend
            "http://localhost:5115",  // TIMS Backend
            "http://localhost:5137"   // EAMS Backend
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// Ajouter les sessions pour stocker les tokens
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseCustomExceptionMiddleware();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    Console.WriteLine("== Migration ==");
    await context.Database.MigrateAsync();

    Console.WriteLine("== Seed Clients ==");
    await ClientApplicationsSeeder.SeedAsync(context);

    Console.WriteLine("== Seed Roles ==");
    await RolesSeeder.SeedAsync(context);

    Console.WriteLine("== Seed Users ==");
    await UsersSeeder.SeedAsync(context);

    Console.WriteLine("== Seed Permissions ==");
    await PermissionsSeeder.SeedAsync(context);

    var permissionCount = await context.Permissions.CountAsync();
    Console.WriteLine($"== Permissions en DB : {permissionCount} ==");

    Console.WriteLine("== Seed RolePermissions ==");
    await RolePermissionsSeeder.SeedAsync(context);

    var roleCount = await context.Roles.CountAsync();
    var rolePermissionCount = await context.RolePermissions.CountAsync();

    Console.WriteLine($"== Roles en DB : {roleCount} ==");
    Console.WriteLine($"== RolePermissions en DB : {rolePermissionCount} ==");
}

Console.WriteLine("== App construite ==");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles(); // Activer les fichiers statiques (CSS, JS)
app.UseCors("AllowClients"); // Activer CORS pour les applications clientes
app.UseSession(); // Activer les sessions
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages(); // Activer Razor Pages
app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        Status = "Healthy",
        Service = "ONEE SSO API",
        Timestamp = DateTime.UtcNow
    });
});

Console.WriteLine("== Avant Run ==");

Console.WriteLine($"Environment : {app.Environment.EnvironmentName}");

foreach (var url in app.Urls)
{
    Console.WriteLine($"URL : {url}");
}
app.Run("http://localhost:5205");