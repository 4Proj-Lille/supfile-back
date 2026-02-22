using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SupFile.Back.Core.Records;
using SupFile.Back.Storage.Configuration;

namespace SupFile.Back.Business.Services;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobStorageSettings _blobSettings;
    
    public BlobService(
        BlobServiceClient blobServiceClient,
        IOptions<BlobStorageSettings> blobSettings)
    {
        _blobServiceClient = blobServiceClient;
        _blobSettings = blobSettings.Value;
    }

    public async Task<Result<Guid>> UploadAsync(Stream stream, string contentType, CancellationToken ct = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobSettings.ContainerName);

        var fileId = Guid.NewGuid();
        var blobClient = containerClient.GetBlobClient(fileId.ToString());

        await blobClient.UploadAsync(
            stream,
            new BlobHttpHeaders { ContentType = contentType },
            cancellationToken: ct);

        return Result.Ok(fileId);
    }

    public async Task<Result<FileResponse>> DownloadAsync(Guid fileId, CancellationToken ct = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobSettings.ContainerName);

        var blobClient = containerClient.GetBlobClient(fileId.ToString());
        try
        {
            Response<BlobDownloadResult> response = await blobClient.DownloadContentAsync(ct);

            return Result.Ok(new FileResponse(response.Value.Content.ToStream(), response.Value.Details.ContentType));
        }
        catch (RequestFailedException e)
        {
            var httpStatusCode = Enum.IsDefined(typeof(HttpStatusCode), e.Status)
                ? (HttpStatusCode)e.Status
                : HttpStatusCode.InternalServerError;

            return Result.Fail(new CustomError(httpStatusCode, e.Message.Split('.').First()));
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid fileId, CancellationToken ct = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobSettings.ContainerName);

        var blobClient = containerClient.GetBlobClient(fileId.ToString());

        var resultResponse = await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
        return resultResponse ? Result.Ok(resultResponse.Value) : Result.Fail(new NotFoundError("File not found"));
    }
}