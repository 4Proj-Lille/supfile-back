namespace SupFile.Back.Core.Configuration;


public class AuthProviderSettings
{
    [Required] public required ProviderSettings Google { get; set; }
}

public class ProviderSettings
{
    [Required] public required string ClientId { get; set; }

    [Required] public required string ClientSecret { get; set; }
}