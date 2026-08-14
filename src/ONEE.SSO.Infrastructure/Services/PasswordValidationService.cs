using System.Text.RegularExpressions;
using ONEE.SSO.Application.Interfaces;

namespace ONEE.SSO.Infrastructure.Services;

public class PasswordValidationService : IPasswordValidationService
{
    public (bool IsValid, string? ErrorMessage) ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return (false, "Le mot de passe ne peut pas être vide.");
        }

        if (password.Length < 8)
        {
            return (false, "Le mot de passe doit contenir au moins 8 caractères.");
        }

        if (password.Length > 128)
        {
            return (false, "Le mot de passe ne peut pas dépasser 128 caractères.");
        }

        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            return (false, "Le mot de passe doit contenir au moins une lettre majuscule.");
        }

        if (!Regex.IsMatch(password, @"[0-9]"))
        {
            return (false, "Le mot de passe doit contenir au moins un chiffre.");
        }

        if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':;{}|<>]"))
        {
            return (false, "Le mot de passe doit contenir au moins un caractère spécial.");
        }

        return (true, null);
    }
}