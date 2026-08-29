using Microsoft.EntityFrameworkCore;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.DTOs.Categorie;
using ONEE.EAMS.Application.Interfaces;
using ONEE.EAMS.Domain.Entities;

namespace ONEE.EAMS.Application.Services;

public class CategorieService : ICategorieService
{
    private readonly IAppDbContext _db;

    public CategorieService(IAppDbContext db) => _db = db;

    public async Task<IEnumerable<CategorieDto>> GetAllAsync()
    {
        return await _db.Categories
            .OrderBy(c => c.Nom)
            .Select(c => new CategorieDto(c.Id, c.Nom, c.Description, c.Icone, c.Couleur, c.Code, c.Equipements.Count))
            .ToListAsync();
    }

    public async Task<CategorieDto> GetByIdAsync(Guid id)
    {
        var c = await _db.Categories.Include(x => x.Equipements).FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException($"Catégorie {id} introuvable.");
        return new CategorieDto(c.Id, c.Nom, c.Description, c.Icone, c.Couleur, c.Code, c.Equipements.Count);
    }

    public async Task<CategorieDto> CreateAsync(CreateCategorieRequest request)
    {
        var cat = new Categorie
        {
            Id = Guid.NewGuid(),
            Nom = request.Nom,
            Description = request.Description,
            Icone = request.Icone,
            Couleur = request.Couleur,
            Code = request.Code.ToUpper()
        };
        _db.Categories.Add(cat);
        await _db.SaveChangesAsync();
        return new CategorieDto(cat.Id, cat.Nom, cat.Description, cat.Icone, cat.Couleur, cat.Code, 0);
    }

    public async Task<CategorieDto> UpdateAsync(Guid id, UpdateCategorieRequest request)
    {
        var cat = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException($"Catégorie {id} introuvable.");
        cat.Nom = request.Nom;
        cat.Description = request.Description;
        cat.Icone = request.Icone;
        cat.Couleur = request.Couleur;
        await _db.SaveChangesAsync();
        var nb = await _db.Equipements.CountAsync(e => e.CategorieId == id);
        return new CategorieDto(cat.Id, cat.Nom, cat.Description, cat.Icone, cat.Couleur, cat.Code, nb);
    }

    public async Task DeleteAsync(Guid id)
    {
        var cat = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new NotFoundException($"Catégorie {id} introuvable.");
        var hasEquipements = await _db.Equipements.AnyAsync(e => e.CategorieId == id);
        if (hasEquipements)
            throw new ConflictException("Cette catégorie est utilisée par des équipements et ne peut pas être supprimée.");
        _db.Categories.Remove(cat);
        await _db.SaveChangesAsync();
    }
}
