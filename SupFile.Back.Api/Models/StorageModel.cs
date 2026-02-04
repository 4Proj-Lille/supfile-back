namespace SupFile.Back.Api.Models;

public class StorageModel
{
    public ICollection<FolderModel> Folders { get; set; }
    
    public ICollection<MediaModel> Medias  { get; set; }
}
