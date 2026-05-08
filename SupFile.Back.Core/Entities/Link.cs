using SupFile.Back.Core.Enums;

namespace SupFile.Back.Core.Entities;

/// <summary>
///     Entity class representing data for table 'Link'.
/// </summary>
public class Link : BaseEntity<Link, int>, IEntity<Link, int>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Link" /> class.
    /// </summary>
    public Link()
    {
    }

    #region Generated Properties

    /// <summary>
    ///     Gets or sets the property value representing column 'Token'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'Token'.
    /// </value>
    public string Token { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the property value representing column Type
    /// </summary>
    /// <value>
    ///     The property value representing column 'Type'.
    /// </value>
    public InvitationItemType Type { get; set; }

    /// <summary>
    ///     Gets or sets the property value representing column 'ExpirationDate'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'ExpirationDate'.
    /// </value>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    ///     Gets or sets the property value representing column 'ShareFileId'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'ShareFileId'.
    /// </value>
    public int? ShareMediaId { get; set; }

    /// <summary>
    ///     Gets or sets the property value representing column 'ShareFolderId'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'ShareFolderId'.
    /// </value>
    public int? ShareFolderId { get; set; }

    public int? TargetUserId { get; set; }

    #endregion

    #region Generated Relationships

    /// <summary>
    ///     Gets or sets the navigation property for entity <see cref="Media" />.
    /// </summary>
    /// <value>
    ///     The navigation property for entity <see cref="Media" />.
    /// </value>
    /// <seealso cref="ShareMediaId" />
    public virtual Media? ShareLinkFile { get; set; }

    /// <summary>
    ///     Gets or sets the navigation property for entity <see cref="Folder" />.
    /// </summary>
    /// <value>
    ///     The navigation property for entity <see cref="Folder" />.
    /// </value>
    /// <seealso cref="ShareFolderId" />
    public virtual Folder? ShareLinkFolder { get; set; }

    public virtual ApplicationUser? TargetUser { get; set; }

    #endregion
}
