namespace ONEE.SSO.Application.Features.Auth.DTOs;

public class JwksDto
{
    public IEnumerable<JwkDto> Keys { get; set; } = new List<JwkDto>();
}

public class JwkDto
{
    public string Kty { get; set; } = string.Empty; // Key Type
    public string Use { get; set; } = string.Empty; // Public Key Use
    public string Kid { get; set; } = string.Empty; // Key ID
    public string Alg { get; set; } = string.Empty; // Algorithm
    public string N { get; set; } = string.Empty;   // Modulus (for RSA)
    public string E { get; set; } = string.Empty;   // Exponent (for RSA)
}