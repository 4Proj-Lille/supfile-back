namespace SupFile.Back.Api.Models;

public class FolderModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int OwnerId { get; set; }

    public int? ParentId { get; set; }
    
    public bool IsActive { get; set; }

}
