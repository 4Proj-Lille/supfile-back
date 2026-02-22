using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using SupFile.Back.Storage.Configuration;
using SupFile.Back.Storage.Interfaces;

namespace SupFile.Back.Storage.Providers;

public class BlobStorageProvider : IStorageProvider
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobStorageSettings _blobSettings;

    public BlobStorageProvider(BlobServiceClient blobServiceClient, IOptions<BlobStorageSettings> blobSettings)
    {
        _blobServiceClient = blobServiceClient;
        _blobSettings = blobSettings.Value;
    }


    public string? GetUrl(string name, string extension, string basePath)
    {
        throw new NotImplementedException();
    }

    public async Task WriteAsync(string name, string extension, byte[] content, bool forceRewrite = false, string baseUrl = "")
    {
        Console.WriteLine($"Content length: {content?.Length}"); // add this
    
        var containerClient = await GetBlobClientAsync(_blobSettings.ContainerName);
        var blobClient = containerClient.GetBlobClient($"{name}{extension}");

        using var stream = new MemoryStream(content);
        const string ContentType = "application/octet-stream";
        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = ContentType });
    }

    public bool Exists(string name, string extension, string? baseUrl = null)
    {
        throw new NotImplementedException();
    }

    public async Task<byte[]> ReadAsync(string name, string extension, string baseUrl = "")
    {
        var containerClient = await GetBlobClientAsync(_blobSettings.ContainerName);
        var blobClient = containerClient.GetBlobClient($"{name}{extension}");
        
        if (!await blobClient.ExistsAsync())
        {
            throw new FileNotFoundException($"Blob {name}{extension} not found in container {_blobSettings.ContainerName}.");
        }
        
        var downloadInfo = await blobClient.DownloadAsync();
        using var ms = new MemoryStream();
        await downloadInfo.Value.Content.CopyToAsync(ms);
        return ms.ToArray();
    }

    private async Task<BlobContainerClient> GetBlobClientAsync(string name)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobSettings.ContainerName);
        await containerClient.CreateIfNotExistsAsync();

        return containerClient;
    }
}
