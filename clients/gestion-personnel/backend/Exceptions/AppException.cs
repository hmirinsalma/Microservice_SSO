namespace GestionPersonnel.API.Exceptions;

public class AppException : Exception
{
    public int StatusCode { get; }

    public AppException(string message, int statusCode = 400) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string resource, object id)
        : base($"{resource} avec l'identifiant '{id}' introuvable.", 404) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message) : base(message, 409) { }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Non autorisé.") : base(message, 401) { }
}
