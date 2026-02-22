namespace SupFile.Back.Storage.Configuration;

public class FileStorageSettings
{
    /// <summary>
    /// Gets or sets the folder path.
    /// </summary>
    /// <value>
    /// The folder path.
    /// </value>
    public string FolderPath { get; set; } = null!;

    /// <summary>
    /// Gets or sets the virtual path.
    /// </summary>
    /// <value>
    /// The virtual path.
    /// </value>
    public string VirtualPath { get; set; } = string.Empty;
}

