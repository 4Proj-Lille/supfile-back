namespace SupFile.Back.Storage.Interfaces;

public interface IStorageProvider
{
    /// <summary>
    /// Gets the public URL of the file, optionally resized.
    /// </summary>
    /// <returns>The public URL of the image.</returns>
    string? GetUrl(string name, string extension, string basePath);

    /// <summary>
    /// Saves the original file and generates resized versions.
    /// </summary>
    Task WriteAsync(string name, string extension, byte[] content, bool forceRewrite = false, string baseUrl = "");

    /// <summary>
    /// Checks if an file exists for the given providerKey and id.
    /// </summary>
    bool Exists(string name, string extension, string? baseUrl = null);

    /// <summary>
    /// Read the file content as a byte array for the given providerKey and id.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="extension"></param>
    /// <param name="baseUrl"></param>
    /// <returns></returns>
    Task<byte[]> ReadAsync(string name, string extension, string baseUrl = "");
}
