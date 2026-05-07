using SupFile.Back.Core.Enums;

namespace SupFile.Back.Core.Dto;

public class UpdateSharePermissionDto
{
    public int ObjectId { get; set; }
    
    public int UserId { get; set; }
    public InvitationItemType Type { get; set; }
    public SharePermission Permission { get; set; }
}
