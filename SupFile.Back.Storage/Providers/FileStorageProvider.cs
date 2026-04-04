using Microsoft.Extensions.Options;
using SupFile.Back.Core.Errors;
using SupFile.Back.Storage.Configuration;
using SupFile.Back.Storage.Interfaces;

namespace SupFile.Back.Storage.Providers;

public class FileStorageProvider : IStorageProvider
{
    private readonly FileStorageSettings _settings;

    public FileStorageProvider(IOptions<FileStorageSettings> settings)
    {
        _settings = settings.Value;
    }

    public Result<string> GetUrl(string name, string extension, string? basePath)
    {
        var folderPath = _settings.FolderPath;
        var virtualPath = _settings.VirtualPath;

        if (basePath is not null)
        {
            folderPath = Path.Combine(folderPath, basePath);
        }

        var files = Directory.EnumerateFiles(folderPath, $"{name}{extension}").ToList();

        if (files.Count == 0)
        {
            return Result.Fail(FileErrors.NotFound(name, extension));
        }

        var file = files.First();
        var ext = Path.GetExtension(file);

        if (basePath is not null)
        {
            virtualPath = Path.Combine(virtualPath, basePath);
        }

        return Result.Ok(Path.Combine(virtualPath, $"{name}{ext}"));
    }

    public async Task<Result<byte[]>> ReadAsync(string name, string extension, string baseUrl = "")
    {
        var physicalPathResult = GetPhysicalPath(name, extension, baseUrl);
        if (physicalPathResult.IsFailed) return physicalPathResult.ToResult();

        var file = await File.ReadAllBytesAsync(physicalPathResult.Value);
        return Result.Ok(file);
    }

    public async Task<Result> WriteAsync(string name, string extension, byte[] content, bool forceRewrite = false,
        string baseUrl = "")
    {
        // Original files are stored in base folder path
        var folderPath = _settings.FolderPath;
        folderPath = Path.Combine(folderPath, baseUrl);

        if (!extension.StartsWith('.'))
        {
            return Result.Fail(FileErrors.ExtensionDoesNotStartWithADot());
        }

        if (content.Length == 0)
        {
            return Result.Fail(FileErrors.EmptyContent());
        }

        var filePath = Path.Combine(folderPath, $"{name}{extension}");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        if (!forceRewrite && File.Exists(filePath))
        {
            return Result.Fail(FileErrors.AlreadyExists(name, extension));
        }

        await File.WriteAllBytesAsync(filePath, content);
        return Result.Ok();
    }

    public Result<bool> Exists(string name, string extension, string? baseUrl = null)
    {
        var basePath = _settings.FolderPath;

        if (baseUrl is not null)
        {
            basePath = Path.Combine(basePath, baseUrl);
        }

        if (!Directory.Exists(basePath))
        {
            return Result.Fail(FileErrors.FileNotFound());
        }

        var filePath = Path.Combine(basePath, $"{name}{extension}");
        return Result.Ok(File.Exists(filePath));
    }

    private Result<string> GetPhysicalPath(string name, string extension, string? subPath = null)
    {
        var basePath = _settings.FolderPath;

        if (subPath is not null && !string.IsNullOrEmpty(subPath))
        {
            basePath = Path.Combine(basePath, subPath);
        }

        var filePath = Path.Combine(basePath, $"{name}{extension}");
        var existsResult = Exists(name, extension, subPath);
        if (existsResult.IsFailed) return existsResult.ToResult();
        if (!existsResult.Value)
        {
            return Result.Fail(FileErrors.FileNotFound());
        }

        return filePath;
    }
    
    public async Task<Result> RenameAsync(string oldName, string newName, string extension)
    {
        var oldFilePathResult = GetPhysicalPath(oldName, extension);
        if (oldFilePathResult.IsFailed) return oldFilePathResult.ToResult();

        var newFilePathResult = GetPhysicalPath(newName, extension);
        if (newFilePathResult.IsSuccess)
        {
            return Result.Fail(FileErrors.AlreadyExists(newName, extension));
        }

        var oldFilePath = oldFilePathResult.Value;
        var newFilePath = Path.Combine(Path.GetDirectoryName(oldFilePath)!, $"{newName}{extension}");

        File.Move(oldFilePath, newFilePath);
        return Result.Ok();
    }
}
