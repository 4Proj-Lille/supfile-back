namespace SupFile.Back.Api.Models;

public class FolderPatchModel
{
    public string Name { get; set; }

    public int OwnerId { get; set; }

    public int? ParentId { get; set; }
}
