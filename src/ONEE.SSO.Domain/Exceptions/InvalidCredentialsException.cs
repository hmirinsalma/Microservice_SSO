namespace ONEE.SSO.Domain.Exceptions;

public sealed class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException()
        : base("Invalid email or password.")
    {
    }
}