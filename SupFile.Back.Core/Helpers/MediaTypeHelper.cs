using SupFile.Back.Core.Enums;

namespace SupFile.Back.Core.Helpers;

public static class MediaTypeHelper
{
    private static readonly Dictionary<string, string> _extensionToType = new(StringComparer.OrdinalIgnoreCase)
    {
        { ".png", nameof(MediaType.Picture) },
        { ".jpg", nameof(MediaType.Picture) },
        { ".jpeg", nameof(MediaType.Picture) },
        { ".jfif", nameof(MediaType.Picture) },
        { ".gif", nameof(MediaType.Picture) },
        { ".webp", nameof(MediaType.Picture) },
        { ".avif", nameof(MediaType.Picture) },
        { ".svg", nameof(MediaType.Picture) },
        { ".bmp", nameof(MediaType.Picture) },
        { ".heic", nameof(MediaType.Picture) },
        { ".tiff", nameof(MediaType.Picture) },
        { ".tif", nameof(MediaType.Picture) },
        { ".ico", nameof(MediaType.Picture) },

        { ".mp4", nameof(MediaType.Video) },
        { ".avi", nameof(MediaType.Video) },
        { ".mov", nameof(MediaType.Video) },
        { ".mkv", nameof(MediaType.Video) },
        { ".webm", nameof(MediaType.Video) },
        { ".wmv", nameof(MediaType.Video) },
        { ".flv", nameof(MediaType.Video) },
        { ".m4v", nameof(MediaType.Video) },
        { ".3gp", nameof(MediaType.Video) },
        { ".ts", nameof(MediaType.Video) },
        { ".ogv", nameof(MediaType.Video) },

        { ".pdf", nameof(MediaType.File) },
        { ".doc", nameof(MediaType.File) },
        { ".docx", nameof(MediaType.File) },
        { ".odt", nameof(MediaType.File) },
        { ".xls", nameof(MediaType.File) },
        { ".xlsx", nameof(MediaType.File) },
        { ".ods", nameof(MediaType.File) },
        { ".ppt", nameof(MediaType.File) },
        { ".pptx", nameof(MediaType.File) },
        { ".odp", nameof(MediaType.File) },
        { ".txt", nameof(MediaType.File) },
        { ".rtf", nameof(MediaType.File) },
        { ".md", nameof(MediaType.File) },
        { ".csv", nameof(MediaType.File) },
        { ".json", nameof(MediaType.File) },
        { ".xml", nameof(MediaType.File) },
        { ".epub", nameof(MediaType.File) },
        { ".zip", nameof(MediaType.File) },
        { ".rar", nameof(MediaType.File) },
        { ".7z", nameof(MediaType.File) },
        { ".tar", nameof(MediaType.File) },
        { ".gz", nameof(MediaType.File) },

        { ".mp3", nameof(MediaType.Audio) },
        { ".wav", nameof(MediaType.Audio) },
        { ".ogg", nameof(MediaType.Audio) },
        { ".opus", nameof(MediaType.Audio) },
        { ".flac", nameof(MediaType.Audio) },
        { ".aac", nameof(MediaType.Audio) },
        { ".m4a", nameof(MediaType.Audio) },
        { ".wma", nameof(MediaType.Audio) },
        { ".aiff", nameof(MediaType.Audio) },
        { ".aif", nameof(MediaType.Audio) },
        { ".mid", nameof(MediaType.Audio) },
        { ".midi", nameof(MediaType.Audio) },
    };

    public static string Resolve(string extension) =>
        _extensionToType.TryGetValue(extension, out var type) ? type : nameof(MediaType.Other);

    public static bool IsSupported(string extension) =>
        _extensionToType.ContainsKey(extension);
    
    public static IEnumerable<string> GetExtensionsByType(MediaType type) =>
        _extensionToType
            .Where(kvp => kvp.Value.Equals(type.ToString(), StringComparison.OrdinalIgnoreCase))
            .Select(kvp => kvp.Key);
    
    public static MediaType GetMediaTypeByExtension(string extension) =>
        _extensionToType.TryGetValue(extension, out var type) && Enum.TryParse<MediaType>(type, true, out var mediaType)
            ? mediaType
            : MediaType.Other;
    
}
