namespace SupFile.Back.Api.Settings;

internal sealed class CorsOptions
{
    public const string PolicyName = "SupChatPolicy";
    public const string SectionName = "Cors";

    public required string[] AllowedOrigins { get; init; }
}
