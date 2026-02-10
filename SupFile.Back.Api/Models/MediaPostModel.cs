namespace SupFile.Back.Api.Models;

public class MediaPostModel
{
    public string Name { get; set; }
    
    public string Extension { get; set; }

    public int Size { get; set; }

    public string Path { get; set; }

    public bool IsActive { get; set; }
    
    public int? FolderId { get; set; }
}
