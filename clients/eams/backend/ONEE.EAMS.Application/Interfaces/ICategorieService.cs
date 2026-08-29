using ONEE.EAMS.Application.DTOs.Categorie;

namespace ONEE.EAMS.Application.Interfaces;

public interface ICategorieService
{
    Task<IEnumerable<CategorieDto>> GetAllAsync();
    Task<CategorieDto> GetByIdAsync(Guid id);
    Task<CategorieDto> CreateAsync(CreateCategorieRequest request);
    Task<CategorieDto> UpdateAsync(Guid id, UpdateCategorieRequest request);
    Task DeleteAsync(Guid id);
}
