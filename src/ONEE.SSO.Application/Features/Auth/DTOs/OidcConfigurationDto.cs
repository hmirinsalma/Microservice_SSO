namespace ONEE.SSO.Application.Features.Auth.DTOs;

public class OidcConfigurationDto
{
    public string Issuer { get; set; } = string.Empty;
    public string AuthorizationEndpoint { get; set; } = string.Empty;
    public string TokenEndpoint { get; set; } = string.Empty;
    public string UserinfoEndpoint { get; set; } = string.Empty;
    public string JwksUri { get; set; } = string.Empty;
    public string EndSessionEndpoint { get; set; } = string.Empty;
    public IEnumerable<string> ResponseTypesSupported { get; set; } = new List<string>();
    public IEnumerable<string> ScopesSupported { get; set; } = new List<string>();
    public IEnumerable<string> GrantTypesSupported { get; set; } = new List<string>();
    public IEnumerable<string> SubjectTypesSupported { get; set; } = new List<string>();
    public IEnumerable<string> IdTokenSigningAlgValuesSupported { get; set; } = new List<string>();
    public IEnumerable<string> TokenEndpointAuthMethodsSupported { get; set; } = new List<string>();
    public IEnumerable<string> ClaimsSupported { get; set; } = new List<string>();
    public bool CodeChallengeMethodsSupported { get; set; } = true;
}