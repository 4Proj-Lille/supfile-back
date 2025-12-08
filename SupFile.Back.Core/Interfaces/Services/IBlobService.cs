using SupFile.Back.Core.Records;

namespace SupFile.Back.Core.Interfaces.Services;

public interface IBlobService
{
    Task<Result<Guid>> UploadAsync(Stream stream, string contentType, CancellationToken ct = default);

    Task<Result<FileResponse>> DownloadAsync(Guid fileId, CancellationToken ct = default);

    Task<Result<bool>> DeleteAsync(Guid fileId, CancellationToken ct = default);
}
