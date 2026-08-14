namespace ONEE.SSO.Application.Interfaces;

public interface IPasswordValidationService
{
    (bool IsValid, string? ErrorMessage) ValidatePassword(string password);
}