namespace SupFile.Back.Core.Configuration;

/// <summary>
///     The settings class for the application.
/// </summary>
public class BlobStorageSettings
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BlobStorageSettings" /> class.
    /// </summary>
    public BlobStorageSettings()
    {
    }

    /// <summary>
    ///     The name of the application.
    /// </summary>
    [Required]
    public required string ContainerName { get; set; }
}
