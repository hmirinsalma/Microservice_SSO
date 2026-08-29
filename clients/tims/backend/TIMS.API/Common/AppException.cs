namespace TIMS.API.Common;

public class AppException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }

    public AppException(string message, int statusCode = 400, string errorCode = "BAD_REQUEST")
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message)
        : base(message, 404, "NOT_FOUND") { }
}

public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "Accès refusé")
        : base(message, 403, "FORBIDDEN") { }
}

public class ConflictException : AppException
{
    public ConflictException(string message, string errorCode = "CONFLICT")
        : base(message, 409, errorCode) { }
}

public class ValidationException : AppException
{
    public List<string> ValidationErrors { get; }
    public ValidationException(List<string> errors)
        : base("Erreur de validation", 422, "VALIDATION_ERROR")
    {
        ValidationErrors = errors;
    }
}
