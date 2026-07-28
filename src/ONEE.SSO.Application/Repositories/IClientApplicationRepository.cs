using ONEE.SSO.Application.Interfaces.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Application.Repositories;

public interface IClientApplicationRepository : IRepository<ClientApplication>
{
    Task<ClientApplication?> GetByClientIdAsync(string clientId);

    Task<bool> ClientExistsAsync(string clientId);
}