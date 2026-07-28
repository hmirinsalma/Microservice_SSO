namespace ONEE.SSO.Domain.Exceptions;

public sealed class DuplicateEmailException : DomainException
{
    public DuplicateEmailException(string email)
        : base($"The email '{email}' already exists.")
    {
    }
}