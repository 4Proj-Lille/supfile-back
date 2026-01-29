namespace SupFile.Back.Core.Configuration;

/// <summary>
///     The settings class for the application.
/// </summary>
public class AppSettings
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppSettings" /> class.
    /// </summary>
    public AppSettings()
    {
        SupportedCultures = new List<string>();
    }

    /// <summary>
    ///     The environment of the application. It can be "Prod" or "Dev".
    /// </summary>
    [Required]
    public required string Environment { get; set; }

    /// <summary>
    ///     The name of the application.
    /// </summary>
    [Required]
    public required string Name { get; set; }

    /// <summary>
    ///     The description of the application.
    /// </summary>
    [Required]
    public required string Description { get; set; }

    /// <summary>
    ///     The version of the application.
    /// </summary>
    [Required]
    public required string Version { get; set; }

    /// <summary>
    ///     The list of supported cultures, by default it is set to English.
    ///     When changing the supported cultures, it is important to also add the corresponding resources files.
    /// </summary>
    [Required]
    public required ICollection<string> SupportedCultures { get; init; }

    /// <summary>
    ///     The default admin email.
    /// </summary>
    [Required]
    public required string DefaultUserEmail { get; set; }

    /// <summary>
    ///     The default admin password.
    /// </summary>
    [Required]
    public required string DefaultUserPassword { get; set; }

    [Required] public required string EmailVerificationFrontendLink { get; set; }
    
    public string LogoUrl { get; set; }
    
    public bool AllowSwagger { get; set; }

    public bool RequireEmailVerification { get; set; }
}
