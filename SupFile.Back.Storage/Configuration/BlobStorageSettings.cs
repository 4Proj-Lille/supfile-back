using System.ComponentModel.DataAnnotations;

namespace SupFile.Back.Storage.Configuration;

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
    
    /// <summary>
    ///     The connection string for the blob storage account.
    /// </summary>
    [Required]
    public required string ConnectionString { get; set; }
}
