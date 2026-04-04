using SupFile.Back.Core.Entities;

namespace SupFile.Back.Core.Entities;

/// <summary>
///     Entity class representing data for table 'Folder'.
/// </summary>
public class Folder : BaseEntity<Folder, int>, IEntity<Folder, int>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Folder" /> class.
    /// </summary>
    public Folder()
    {
        #region Generated Constructor
        
        Medias = new HashSet<Media>();
        Links = new HashSet<Link>();
        ParentFolders = new HashSet<Folder>();
        Shares = new HashSet<Share>();

        #endregion
    }

    #region Generated Properties

    /// <summary>
    ///     Gets or sets the property value representing column 'Name'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'Name'.
    /// </value>
    public string Name { get; set; } = null!;
    
    /// <summary>
    ///     Gets or sets the property value representing column 'OwnerId'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'OwnerId'.
    /// </value>
    public int OwnerId { get; set; }

    /// <summary>
    ///     Gets or sets the property value representing column 'ParentId'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'ParentId'.
    /// </value>
    public int? ParentId { get; set; }
    
    public bool IsActive { get; set; } = true;

    #endregion
    
    #region Generated Relationships
    
    /// <summary>
    ///     Gets or sets the navigation property for entity <see cref="ApplicationUser" />.
    /// </summary>
    /// <value>
    ///     The navigation property for entity <see cref="ApplicationUser" />.
    /// </value>
    /// <seealso cref="OwnerId" />
    public virtual ApplicationUser OwnerApplicationUserFolder { get; set; } = null!;
    
    /// <summary>
    ///     Gets or sets the navigation property for entity <see cref="Folder" />.
    /// </summary>
    /// <value>
    ///     The navigation property for entity <see cref="Folder" />.
    /// </value>
    /// <seealso cref="ParentId" />
    public virtual Folder? ParentFolder { get; set; }
    
    /// <summary>
    ///     Gets or sets the navigation collection for entity <see cref="Folder" />.
    /// </summary>
    /// <value>
    ///     The navigation collection for entity <see cref="Folder" />.
    /// </value>
    public virtual ICollection<Folder> ParentFolders { get; }
    
    /// <summary>
    ///     Gets or sets the navigation collection for entity <see cref="Media" />.
    /// </summary>
    /// <value>
    ///     The navigation collection for entity <see cref="Media" />.
    /// </value>
    public virtual ICollection<Media> Medias { get; }
    
    /// <summary>
    ///     Gets or sets the navigation collection for entity <see cref="Link" />.
    /// </summary>
    /// <value>
    ///     The navigation collection for entity <see cref="Link" />.
    /// </value>
    public virtual ICollection<Link> Links { get; }
    
    /// <summary>
    ///     Gets or sets the navigation collection for entity <see cref="Share" />.
    /// </summary>
    /// <value>
    ///     The navigation collection for entity <see cref="Share" />.
    /// </value>
    public virtual ICollection<Share> Shares { get; }
    
    #endregion
}
