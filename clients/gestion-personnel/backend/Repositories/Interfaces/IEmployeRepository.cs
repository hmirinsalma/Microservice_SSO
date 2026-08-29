using GestionPersonnel.API.DTOs.Employe;
using GestionPersonnel.API.Models;

namespace GestionPersonnel.API.Repositories.Interfaces;

public interface IEmployeRepository
{
    Task<(IEnumerable<Employe> Items, int TotalCount)> GetPagedAsync(EmployeQueryDto query);
    Task<Employe?> GetByIdAsync(int id);
    Task<Employe?> GetByMatriculeAsync(string matricule);
    Task<Employe?> GetByEmailAsync(string email);
    Task<Employe> CreateAsync(Employe employe);
    Task<Employe> UpdateAsync(Employe employe);
    Task DeleteAsync(Employe employe);
    Task<IEnumerable<Employe>> GetLastAddedAsync(int count);
}
