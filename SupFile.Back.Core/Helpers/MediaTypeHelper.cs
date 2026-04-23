namespace SupFile.Back.Core.Helpers;

public static class MediaTypeHelper
{
    private static readonly Dictionary<string, string> _extensionToType = new(StringComparer.OrdinalIgnoreCase)
    {
        { ".png", "Picture" },
        { ".jpg", "Picture" },
        { ".jpeg", "Picture" },
        { ".gif", "Picture" },
        { ".webp", "Picture" },
        { ".svg", "Picture" },
        { ".bmp", "Picture" },

        { ".mp4", "Video" },
        { ".avi", "Video" },
        { ".mov", "Video" },
        { ".mkv", "Video" },
        { ".webm", "Video" },

        { ".pdf", "File" },
        { ".doc", "File" },
        { ".docx", "File" },
        { ".xls", "File" },
        { ".xlsx", "File" },
        { ".ppt", "File" },
        { ".pptx", "File" },
        { ".txt", "File" },
        { ".csv", "File" },

        { ".mp3", "Audio" },
        { ".wav", "Audio" },
        { ".ogg", "Audio" },
        { ".flac", "Audio" },
        { ".aac", "Audio" },
        { ".m4a", "Audio" },
        { ".wma", "Audio" },
    };

    public static string Resolve(string extension) =>
        _extensionToType.TryGetValue(extension, out var type) ? type : "Other";

    public static bool IsSupported(string extension) =>
        _extensionToType.ContainsKey(extension);
}
