using System.Text.RegularExpressions;

namespace ONEE.SSO.Domain.ValueObjects;

public sealed record Email
{
    public string Value { get; }

    public Email(string value)
    {
        value = value.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email is required.");

        if (!Regex.IsMatch(
                value,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException("Invalid email.");

        Value = value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email)
        => email.Value;

    public static implicit operator Email(string value)
        => new(value);
}