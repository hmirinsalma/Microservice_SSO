namespace ONEE.SSO.Shared.Settings;

public class CorsSettings
{
    public const string SectionName = "Cors";

    public List<string> AllowedOrigins { get; set; } = [];
}