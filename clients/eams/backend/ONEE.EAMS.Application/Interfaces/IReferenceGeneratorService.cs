namespace ONEE.EAMS.Application.Interfaces;

public interface IReferenceGeneratorService
{
    Task<string> GenerateAsync(string categoryCode);
}
