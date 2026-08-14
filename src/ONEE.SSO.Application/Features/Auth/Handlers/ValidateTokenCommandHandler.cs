using System.Security.Claims;
using ONEE.SSO.Application.Features.Auth.Commands;
using ONEE.SSO.Application.Features.Auth.DTOs;
using ONEE.SSO.Application.Interfaces;
using ONEE.SSO.Application.Repositories;

namespace ONEE.SSO.Application.Features.Auth.Handlers;

public class ValidateTokenCommandHandler
{
    private readonly IJwtService _jwtService;
    private readonly IJwtBlocklistService _jwtBlocklistService;
    private readonly IUserRepository _userRepository;

    public ValidateTokenCommandHandler(
        IJwtService jwtService,
        IJwtBlocklistService jwtBlocklistService,
        IUserRepository userRepository)
    {
        _jwtService = jwtService;
        _jwtBlocklistService = jwtBlocklistService;
        _userRepository = userRepository;
    }

    public async Task<ValidateTokenResponseDto> HandleAsync(ValidateTokenCommand command)
    {
        if (string.IsNullOrEmpty(command.Token))
        {
            return new ValidateTokenResponseDto
            {
                IsValid = false,
                Reason = "Token absent"
            };
        }

        // Valider la structure et la signature du JWT
        var principal = _jwtService.ValidateToken(command.Token);
        if (principal == null)
        {
            return new ValidateTokenResponseDto
            {
                IsValid = false,
                Reason = "Token invalide ou expiré"
            };
        }

        // Vérifier si le token est dans la blocklist (révoqué)
        var jti = _jwtService.GetJtiFromToken(command.Token);
        if (!string.IsNullOrEmpty(jti))
        {
            var isRevoked = await _jwtBlocklistService.IsTokenRevokedAsync(jti);
            if (isRevoked)
            {
                return new ValidateTokenResponseDto
                {
                    IsValid = false,
                    Reason = "Token révoqué"
                };
            }
        }

        // Extraire les informations du token
        var userIdClaim = principal.FindFirst("sub")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return new ValidateTokenResponseDto
            {
                IsValid = false,
                Reason = "Token malformé - UserId invalide"
            };
        }

        // Vérifier que l'utilisateur existe toujours et est actif
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !user.IsActive)
        {
            return new ValidateTokenResponseDto
            {
                IsValid = false,
                Reason = "Utilisateur introuvable ou inactif"
            };
        }

        // Extraire les claims
        var email = principal.FindFirst("email")?.Value ?? user.Email;
        var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var permissions = principal.FindAll("permission").Select(c => c.Value).ToList();

        return new ValidateTokenResponseDto
        {
            IsValid = true,
            UserId = userId,
            Email = email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles,
            Permissions = permissions
        };
    }
}