using SupFile.Back.Core.Enums;

namespace SupFile.Back.Api.Models;

public class PendingInvitationModel
{
    public int Id { get; set; }
    public string Token { get; set; } = null!;
    public InvitationItemType Type { get; set; }
    public DateTime ExpirationDate { get; set; }
    public int? ShareMediaId { get; set; }
    public int? ShareFolderId { get; set; }
    public int? TargetUserId { get; set; }
}
