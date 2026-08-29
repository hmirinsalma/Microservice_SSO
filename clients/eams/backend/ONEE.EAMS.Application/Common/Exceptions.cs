namespace ONEE.EAMS.Application.Common;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "Accès refusé.") : base(message) { }
}

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Non autorisé.") : base(message) { }
}

public class ValidationException : Exception
{
    public List<string> Errors { get; }
    public ValidationException(IEnumerable<string> errors) : base("Validation échouée.")
    {
        Errors = errors.ToList();
    }
}
