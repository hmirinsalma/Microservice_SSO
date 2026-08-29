using GestionPersonnel.API.DTOs.Common;
using GestionPersonnel.API.DTOs.Employe;

namespace GestionPersonnel.API.Services.Interfaces;

public interface IEmployeService
{
    Task<PagedResultDto<EmployeDto>> GetPagedAsync(EmployeQueryDto query);
    Task<EmployeDto> GetByIdAsync(int id);
    Task<EmployeDto> CreateAsync(CreateEmployeDto dto);
    Task<EmployeDto> UpdateAsync(int id, UpdateEmployeDto dto);
    Task DeleteAsync(int id);
}
