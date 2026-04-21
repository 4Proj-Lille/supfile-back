namespace SupFile.Back.Api.Models;

public class MediaModel
{
    public int Id { get; set; }

    public string Name { get; set; }
    
    public string Extension { get; set; }

    public int Size { get; set; }

    public bool IsActive { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime UpdatedDate { get; set; }
    
    public int? FolderId { get; set; }

    public int OwnerId { get; set; }
    
    public Guid UniqueId { get; set; }
}
