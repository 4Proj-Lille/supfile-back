using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using SupFile.Back.Core.Errors;
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


    public Result<string> GetUrl(string name, string extension, string? basePath)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobSettings.ContainerName);
        var blobClient = containerClient.GetBlobClient($"{name}{extension}");
        var url = blobClient.Uri.ToString();
        return Result.Ok(url);
    }

    public async Task<Result> WriteAsync(string name, string extension, byte[] content, bool forceRewrite = false,
        string baseUrl = "")
    {
        var existsResult = Exists(name, extension, baseUrl);
        if (existsResult.IsFailed) return existsResult.ToResult();
        if (existsResult.Value && !forceRewrite)
        {
            return Result.Fail(FileErrors.AlreadyExists(name, extension));
        }

        var containerClient = await GetBlobClientAsync(_blobSettings.ContainerName);
        var blobClient = containerClient.GetBlobClient($"{name}{extension}");

        using var stream = new MemoryStream(content);
        const string ContentType = "application/octet-stream";
        var response = await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = ContentType });
        if (!response.HasValue) return Result.Fail(FileErrors.FileUploadFailed());

        return Result.Ok();
    }

    public Result<bool> Exists(string name, string extension, string? baseUrl = null)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobSettings.ContainerName);
        var blobClient = containerClient.GetBlobClient($"{name}{extension}");
        var exists = blobClient.Exists();
        return Result.Ok(exists.Value);
    }

    public async Task<Result<byte[]>> ReadAsync(string name, string extension, string baseUrl = "")
    {
        var containerClient = await GetBlobClientAsync(_blobSettings.ContainerName);
        var blobClient = containerClient.GetBlobClient($"{name}{extension}");

        var existsResult = Exists(name, extension, baseUrl);
        if (existsResult.IsFailed) return existsResult.ToResult();
        if (!existsResult.Value)
        {
            return Result.Fail(FileErrors.FileNotFound());
        }

        var downloadInfo = await blobClient.DownloadAsync();
        using var ms = new MemoryStream();

        await downloadInfo.Value.Content.CopyToAsync(ms);
        return Result.Ok(ms.ToArray());
    }

    private async Task<BlobContainerClient> GetBlobClientAsync(string name)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobSettings.ContainerName);
        await containerClient.CreateIfNotExistsAsync();

        return containerClient;
    }
}
