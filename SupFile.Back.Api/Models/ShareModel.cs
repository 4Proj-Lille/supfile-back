namespace SupFile.Back.Api.Models;

public class ShareModel
{

    public string Permission { get; set; } = null!;
    
    public int UserId { get; set; }
    
    public string Type { get; set; } = null!;
    
    public int? ShareMediaId { get; set; }
    
    public int? ShareFolderId { get; set; }
}