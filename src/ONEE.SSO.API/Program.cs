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

Console.WriteLine("== Début Program ==");


var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("== Builder créé ==");
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
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