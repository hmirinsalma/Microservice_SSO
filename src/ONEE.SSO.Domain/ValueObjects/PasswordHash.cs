namespace ONEE.SSO.Domain.ValueObjects;

public sealed record PasswordHash
{
    public string Value { get; }

    public PasswordHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Password hash is required.");

        Value = value;
    }

    public override string ToString()
        => Value;
}