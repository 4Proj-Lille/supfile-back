namespace SupFile.Back.Api.Models;

public class MediaPatchModel
{
    public string? Name { get; set; }
    
    public string? Extension { get; set; }

    public int? Size { get; set; }
    
    public bool? IsActive { get; set; }
    
    public int? FolderId { get; set; }

    public int OwnerId { get; set; }
}
