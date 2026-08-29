using Microsoft.EntityFrameworkCore;
using ONEE.EAMS.Application.Interfaces;
using ONEE.EAMS.Infrastructure.Data;

namespace ONEE.EAMS.Infrastructure.Services;

public class ReferenceGeneratorService : IReferenceGeneratorService
{
    private readonly AppDbContext _context;
    private static readonly SemaphoreSlim _lock = new(1, 1);

    public ReferenceGeneratorService(AppDbContext context) => _context = context;

    public async Task<string> GenerateAsync(string categoryCode)
    {
        await _lock.WaitAsync();
        try
        {
            var year = DateTime.UtcNow.Year;
            var prefix = $"{categoryCode.ToUpper()}-{year}-";
            var count = await _context.Equipements
                .Where(e => e.Reference.StartsWith(prefix))
                .CountAsync();
            return $"{prefix}{(count + 1):D5}";
        }
        finally
        {
            _lock.Release();
        }
    }
}
