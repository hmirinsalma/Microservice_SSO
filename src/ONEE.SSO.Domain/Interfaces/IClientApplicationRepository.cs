using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Domain.Interfaces;

public interface IClientApplicationRepository
{
    Task<ClientApplication?> GetByClientIdAsync(string clientId);

    Task<IEnumerable<ClientApplication>> GetAllAsync();

    Task AddAsync(ClientApplication client);

    Task UpdateAsync(ClientApplication client);
}