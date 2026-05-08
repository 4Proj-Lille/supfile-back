using SupFile.Back.Core.Configuration;
using SupFile.Back.Core.Enums;

namespace SupFile.Back.Business.Models.Templates.Emails;

public class ShareInvitationEmailModel
{
    public string InvitedUserName { get; set; }
    public string InviterUserName { get; set; }
    public InvitationItemType ItemType { get; set; }
    public string ShareLink { get; set; }
    public AppSettings AppSettings { get; set; }
}
