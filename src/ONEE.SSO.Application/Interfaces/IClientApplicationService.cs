using ONEE.SSO.Application.DTOs;

namespace ONEE.SSO.Application.Interfaces;

public interface IClientApplicationService
{
    Task<IEnumerable<ClientApplicationDto>> GetAllAsync();

    Task<ClientApplicationDto?> GetByIdAsync(Guid id);

    Task<ClientApplicationDto> CreateAsync(CreateClientApplicationDto dto);

    Task<ClientApplicationDto> UpdateAsync(Guid id, UpdateClientApplicationDto dto);

    Task DeleteAsync(Guid id);
}