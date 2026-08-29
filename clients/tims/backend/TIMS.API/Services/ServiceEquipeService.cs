using Microsoft.EntityFrameworkCore;
using TIMS.API.Common;
using TIMS.API.Data;
using TIMS.API.DTOs.User;
using TIMS.API.Interfaces;

namespace TIMS.API.Services;

public class ServiceEquipeService : IServiceEquipeService
{
    private readonly ApplicationDbContext _db;
    public ServiceEquipeService(ApplicationDbContext db) { _db = db; }

    public async Task<List<ServiceDto>> GetAllServicesAsync()
    {
        var services = await _db.Services
            .Include(s => s.Users).Include(s => s.Equipes)
            .Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync();
        return services.Select(s => new ServiceDto
        {
            Id = s.Id, Name = s.Name, Description = s.Description,
            IsActive = s.IsActive, UserCount = s.Users.Count, EquipeCount = s.Equipes.Count
        }).ToList();
    }

    public async Task<ServiceDto> GetServiceByIdAsync(int id)
    {
        var s = await _db.Services.Include(x => x.Users).Include(x => x.Equipes)
            .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NotFoundException("Service introuvable");
        return new ServiceDto
        {
            Id = s.Id, Name = s.Name, Description = s.Description,
            IsActive = s.IsActive, UserCount = s.Users.Count, EquipeCount = s.Equipes.Count
        };
    }

    public async Task<ServiceDto> CreateServiceAsync(CreateServiceDto dto)
    {
        var s = new Entities.Service { Name = dto.Name, Description = dto.Description };
        _db.Services.Add(s);
        await _db.SaveChangesAsync();
        return await GetServiceByIdAsync(s.Id);
    }

    public async Task<ServiceDto> UpdateServiceAsync(int id, CreateServiceDto dto)
    {
        var s = await _db.Services.FindAsync(id) ?? throw new NotFoundException("Service introuvable");
        s.Name = dto.Name; s.Description = dto.Description;
        await _db.SaveChangesAsync();
        return await GetServiceByIdAsync(id);
    }

    public async Task<List<EquipeDto>> GetAllEquipesAsync()
    {
        var equipes = await _db.Equipes.Include(e => e.Service).Include(e => e.Members)
            .Where(e => e.IsActive).OrderBy(e => e.Name).ToListAsync();
        return equipes.Select(MapEquipe).ToList();
    }

    public async Task<List<EquipeDto>> GetEquipesByServiceAsync(int serviceId)
    {
        var equipes = await _db.Equipes.Include(e => e.Service).Include(e => e.Members)
            .Where(e => e.ServiceId == serviceId && e.IsActive).ToListAsync();
        return equipes.Select(MapEquipe).ToList();
    }

    public async Task<EquipeDto> CreateEquipeAsync(CreateEquipeDto dto)
    {
        var e = new Entities.Equipe { Name = dto.Name, Description = dto.Description, ServiceId = dto.ServiceId };
        _db.Equipes.Add(e);
        await _db.SaveChangesAsync();
        return MapEquipe(await _db.Equipes.Include(x => x.Service).Include(x => x.Members).FirstAsync(x => x.Id == e.Id));
    }

    public async Task<EquipeDto> UpdateEquipeAsync(int id, CreateEquipeDto dto)
    {
        var e = await _db.Equipes.FindAsync(id) ?? throw new NotFoundException("Équipe introuvable");
        e.Name = dto.Name; e.Description = dto.Description; e.ServiceId = dto.ServiceId;
        await _db.SaveChangesAsync();
        return MapEquipe(await _db.Equipes.Include(x => x.Service).Include(x => x.Members).FirstAsync(x => x.Id == id));
    }

    private static EquipeDto MapEquipe(Entities.Equipe e) => new()
    {
        Id = e.Id, Name = e.Name, Description = e.Description, IsActive = e.IsActive,
        ServiceId = e.ServiceId, ServiceName = e.Service?.Name, MemberCount = e.Members.Count
    };
}
