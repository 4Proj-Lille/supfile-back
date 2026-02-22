using SupFile.Back.Storage.Configuration;
using SupFile.Back.Storage.Interfaces;

namespace SupFile.Back.Storage.Providers;

public class BlobStorageProvider : IStorageProvider
{
    private readonly StorageSettings _settings;

    public BlobStorageProvider(StorageSettings settings)
    {
        _settings = settings;
    }

    
    public string? GetUrl(string name, string extension, string basePath)
    {
        throw new NotImplementedException();
    }

    public Task WriteAsync(string name, string extension, byte[] content, bool forceRewrite = false, string baseUrl = "")
    {
        throw new NotImplementedException();
    }

    public bool Exists(string name, string extension, string? baseUrl = null)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> ReadAsync(string name, string extension, string baseUrl = "")
    {
        throw new NotImplementedException();
    }
}
