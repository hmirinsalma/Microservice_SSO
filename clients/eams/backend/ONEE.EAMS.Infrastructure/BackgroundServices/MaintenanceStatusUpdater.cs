using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ONEE.EAMS.Application.Interfaces;

namespace ONEE.EAMS.Infrastructure.BackgroundServices;

public class MaintenanceStatusUpdater : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<MaintenanceStatusUpdater> _logger;

    public MaintenanceStatusUpdater(IServiceProvider services, ILogger<MaintenanceStatusUpdater> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Attendre 30 secondes après le démarrage pour ne pas ralentir l'initialisation
        try { await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); }
        catch (OperationCanceledException) { return; }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var maintenanceService = scope.ServiceProvider.GetRequiredService<IMaintenanceService>();
                var notifService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                await maintenanceService.UpdateRetardStatusAsync();
                await notifService.CheckGarantieExpirationsAsync();

                _logger.LogInformation("MaintenanceStatusUpdater: mise à jour à {Time}", DateTime.UtcNow);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur dans MaintenanceStatusUpdater");
            }

            try { await Task.Delay(TimeSpan.FromHours(1), stoppingToken); }
            catch (OperationCanceledException) { break; }
        }
    }
}
