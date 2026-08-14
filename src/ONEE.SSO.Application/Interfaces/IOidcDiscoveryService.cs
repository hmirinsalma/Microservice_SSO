using ONEE.SSO.Application.Features.Auth.DTOs;

namespace ONEE.SSO.Application.Interfaces;

public interface IOidcDiscoveryService
{
    OidcConfigurationDto GetOidcConfiguration(string baseUrl);
    JwksDto GetJwks();
}