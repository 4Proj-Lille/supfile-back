namespace SupFile.Back.Core.Configuration;

/// <summary>
///     The settings class for the application.
/// </summary>
public class FrontEndSettings
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppSettings" /> class.
    /// </summary>
    public FrontEndSettings()
    {
    }

    [Required] public required string BaseUrl { get; set; }

    [Required] public required string EmailVerificationLink { get; set; }

    [Required] public required string ResetPasswordLink { get; set; }
    [Required] public required string ShareLink { get; set; }
}
