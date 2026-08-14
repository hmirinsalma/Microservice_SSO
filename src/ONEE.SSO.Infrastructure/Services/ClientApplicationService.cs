using ONEE.SSO.Application.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;
using ONEE.SSO.Domain.Entities;

namespace ONEE.SSO.Infrastructure.Services;

public class ClientApplicationService : IClientApplicationService
{
    private readonly IClientApplicationRepository _clientRepository;

    public ClientApplicationService(IClientApplicationRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    private static ClientApplicationDto MapToDto(ClientApplication client)
    {
        return new ClientApplicationDto
        {
            Id = client.Id,
            Name = client.Name,
            ClientId = client.ClientId,
            RedirectUri = client.RedirectUri,
            PostLogoutRedirectUri = client.PostLogoutRedirectUri,
            AllowedScopes = client.AllowedScopes,
            AllowedGrantTypes = client.AllowedGrantTypes,
            RequirePkce = client.RequirePkce,
            RequireConsent = client.RequireConsent,
            AccessTokenLifetime = client.AccessTokenLifetime,
            RefreshTokenLifetime = client.RefreshTokenLifetime,
            IsActive = client.IsActive
        };
    }

    public async Task<IEnumerable<ClientApplicationDto>> GetAllAsync()
    {
        var clients = await _clientRepository.GetAllAsync();

        return clients.Select(MapToDto);
    }

    public async Task<ClientApplicationDto?> GetByIdAsync(Guid id)
    {
        var client = await _clientRepository.GetByIdAsync(id);

        return client is null ? null : MapToDto(client);
    }

    public async Task<ClientApplicationDto> CreateAsync(CreateClientApplicationDto dto)
    {
        if (await _clientRepository.ClientExistsAsync(dto.ClientId))
            throw new InvalidOperationException("Client already exists.");

        var client = new ClientApplication
        {
            Id = Guid.NewGuid(),

            Name = dto.Name,

            ClientId = dto.ClientId,

            ClientSecret = dto.ClientSecret,

            RedirectUri = dto.RedirectUri,

            PostLogoutRedirectUri = dto.PostLogoutRedirectUri,

            AllowedScopes = dto.AllowedScopes,

            AllowedGrantTypes = dto.AllowedGrantTypes,

            RequirePkce = dto.RequirePkce,

            RequireConsent = dto.RequireConsent,

            AccessTokenLifetime = dto.AccessTokenLifetime,

            RefreshTokenLifetime = dto.RefreshTokenLifetime,

            IsActive = dto.IsActive,

            CreatedAt = DateTime.UtcNow
        };

        await _clientRepository.AddAsync(client);
        await _clientRepository.SaveChangesAsync();

        return MapToDto(client);
    }

    public async Task<ClientApplicationDto> UpdateAsync(Guid id, UpdateClientApplicationDto dto)
    {
        var client = await _clientRepository.GetByIdAsync(id);

        if (client is null)
            throw new KeyNotFoundException("Client application not found.");

        client.Name = dto.Name;

        client.RedirectUri = dto.RedirectUri;

        client.PostLogoutRedirectUri = dto.PostLogoutRedirectUri;

        client.AllowedScopes = dto.AllowedScopes;

        client.AllowedGrantTypes = dto.AllowedGrantTypes;

        client.RequirePkce = dto.RequirePkce;

        client.RequireConsent = dto.RequireConsent;

        client.AccessTokenLifetime = dto.AccessTokenLifetime;

        client.RefreshTokenLifetime = dto.RefreshTokenLifetime;

        client.IsActive = dto.IsActive;

        client.UpdatedAt = DateTime.UtcNow;

        _clientRepository.Update(client);
        await _clientRepository.SaveChangesAsync();

        return MapToDto(client);
    }

    public async Task DeleteAsync(Guid id)
    {
        var client = await _clientRepository.GetByIdAsync(id);

        if (client is null)
            throw new KeyNotFoundException("Client application not found.");

        _clientRepository.Delete(client);
        await _clientRepository.SaveChangesAsync();
    }
    public async Task<IEnumerable<ClientApplicationDto>> SearchAsync(string keyword)
    {
        var clients = await _clientRepository.SearchAsync(keyword);

        return clients.Select(MapToDto);
    }
    public async Task<IEnumerable<ClientApplicationDto>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var clients = await _clientRepository.GetPagedAsync(pageNumber, pageSize);

        return clients.Select(MapToDto);
    }
    public async Task ActivateAsync(Guid id)
    {
        var client = await _clientRepository.GetByIdAsync(id);

        if (client is null)
            throw new KeyNotFoundException("Client application not found.");

        client.IsActive = true;
        client.UpdatedAt = DateTime.UtcNow;

        _clientRepository.Update(client);
        await _clientRepository.SaveChangesAsync();
    }
    public async Task DeactivateAsync(Guid id)
    {
        var client = await _clientRepository.GetByIdAsync(id);

        if (client is null)
            throw new KeyNotFoundException("Client application not found.");

        client.IsActive = false;
        client.UpdatedAt = DateTime.UtcNow;

        _clientRepository.Update(client);
        await _clientRepository.SaveChangesAsync();
    }
}