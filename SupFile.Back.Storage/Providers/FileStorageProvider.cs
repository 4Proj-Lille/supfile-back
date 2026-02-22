using Microsoft.Extensions.Options;
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

    public string? GetUrl(string name, string extension, string subPath)
    {
        var folderPath = _settings.FolderPath;
        var virtualPath = _settings.VirtualPath;

        if (subPath is not null)
        {
            folderPath = Path.Combine(folderPath, subPath);
        }

        // Look for any file that matches id.*
        var files = Directory.EnumerateFiles(folderPath, $"{name}{extension}").ToList();

        if (files.Count == 0)
        {
            return null; // no file found
        }

        // Take the first match
        var file = files.First();
        var ext = Path.GetExtension(file);

        if (subPath is not null)
        {
            virtualPath = Path.Combine(virtualPath, subPath);
        }

        return Path.Combine(virtualPath, $"{name}{ext}");
    }

    public async Task<byte[]> ReadAsync(string name, string extension, string subPath = "")
    {
        var physicalPath = GetPhysicalPath(name, extension, subPath);

        return await File.ReadAllBytesAsync(physicalPath);
    }

    public async Task WriteAsync(string name, string extension, byte[] content, bool forceRewrite = false, string baseUrl = "")
    {
        // Original files are stored in base folder path
        var folderPath = _settings.FolderPath;
        folderPath = Path.Combine(folderPath, baseUrl);

        if (!extension.StartsWith("."))
        {
            throw new InvalidDataException("Extension must start with a dot.");
        }

        if (content == null || content.Length == 0)
        {
            throw new InvalidDataException("File content cannot be empty.");
        }
        
        var filePath = Path.Combine(folderPath, $"{name}{extension}");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        if (!forceRewrite && File.Exists(filePath))
        {
            return;
        }

        await File.WriteAllBytesAsync(filePath, content);
    }

    public bool Exists(string name, string extension, string? baseUrl = null)
    {
        var basePath = _settings.FolderPath;

        if (baseUrl is not null)
        {
            basePath = Path.Combine(basePath, baseUrl);
        }

        if (!Directory.Exists(basePath))
        {
            return false;
        }

        var filePath = Path.Combine(basePath, $"{name}{extension}");
        return File.Exists(filePath);
    }

    private string GetPhysicalPath(string name, string extension, string? subPath = null)
    {
        var basePath = _settings.FolderPath;

        if (subPath is not null && !string.IsNullOrEmpty(subPath))
        {
            basePath = Path.Combine(basePath, subPath);
        }

        var filePath = Path.Combine(basePath, $"{name}{extension}");
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File {filePath} not found.");
        }

        return filePath;
    }
}
