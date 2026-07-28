using ONEE.SSO.API.Extensions;
using ONEE.SSO.Application.DependencyInjection;
using ONEE.SSO.Infrastructure.DependencyInjection;
using Serilog;

Console.WriteLine("== Début Program ==");

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("== Builder créé ==");

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