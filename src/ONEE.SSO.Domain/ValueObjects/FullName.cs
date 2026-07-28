namespace ONEE.SSO.Domain.ValueObjects;

public sealed record FullName
{
    public string FirstName { get; }

    public string LastName { get; }

    public FullName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.");

        FirstName = firstName.Trim();

        LastName = lastName.Trim();
    }

    public override string ToString()
        => $"{FirstName} {LastName}";
}