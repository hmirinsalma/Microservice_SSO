using ONEE.SSO.Application.DTOs;

namespace ONEE.SSO.Application.Interfaces;

public interface IClientApplicationService
{
    Task<IEnumerable<ClientApplicationDto>> GetAllAsync();

    Task<ClientApplicationDto?> GetByIdAsync(Guid id);

    Task<ClientApplicationDto> CreateAsync(CreateClientApplicationDto dto);

    Task<ClientApplicationDto> UpdateAsync(Guid id, UpdateClientApplicationDto dto);

    Task DeleteAsync(Guid id);

    Task<IEnumerable<ClientApplicationDto>> SearchAsync(string keyword);

    Task<IEnumerable<ClientApplicationDto>> GetPagedAsync(int page, int pageSize);

    Task ActivateAsync(Guid id);

    Task DeactivateAsync(Guid id);
}