namespace ONEE.SSO.Application.Features.Auth.DTOs;

public class ValidateTokenResponseDto
{
    public bool IsValid { get; set; }
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public IEnumerable<string> Permissions { get; set; } = new List<string>();
    public string? Reason { get; set; }
}