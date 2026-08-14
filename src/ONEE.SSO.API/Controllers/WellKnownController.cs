using Microsoft.AspNetCore.Mvc;
using ONEE.SSO.Application.Interfaces;

namespace ONEE.SSO.API.Controllers;

[ApiController]
[Route(".well-known")]
public class WellKnownController : ControllerBase
{
    private readonly IOidcDiscoveryService _oidcDiscoveryService;

    public WellKnownController(IOidcDiscoveryService oidcDiscoveryService)
    {
        _oidcDiscoveryService = oidcDiscoveryService;
    }

    /// <summary>
    /// OIDC Discovery endpoint - Retourne la configuration OpenID Connect du serveur
    /// </summary>
    [HttpGet("openid-configuration")]
    [Produces("application/json")]
    public IActionResult GetOpenIdConfiguration()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var configuration = _oidcDiscoveryService.GetOidcConfiguration(baseUrl);
        
        return Ok(configuration);
    }

    /// <summary>
    /// JWKS endpoint - Retourne les clés publiques pour la validation des JWT
    /// </summary>
    [HttpGet("jwks.json")]
    [Produces("application/json")]
    public IActionResult GetJwks()
    {
        var jwks = _oidcDiscoveryService.GetJwks();
        return Ok(jwks);
    }
}