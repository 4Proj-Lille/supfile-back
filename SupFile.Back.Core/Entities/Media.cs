namespace SupFile.Back.Core.Entities;

/// <summary>
///     Entity class representing data for table 'Media'.
/// </summary>
public class Media : BaseEntity<Media, int>, IEntity<Media, int>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Media" /> class.
    /// </summary>
    public Media()
    {
        #region Generated Constructor
        
        Links = new HashSet<Link>();
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
    public string Extension { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the property value representing column 'Size'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'Size'.
    /// </value>
    public int Size { get; set; }
    
    /// <summary>
    ///     Gets or sets the property value representing column 'Path'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'Path'.
    /// </value>
    public int Path { get; set; }
    
    /// <summary>
    ///     Gets or sets the property value representing column 'IsActive'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'IsActive'.
    /// </value>
    public bool IsActive { get; set; }
    
    /// <summary>
    ///     Gets or sets the property value representing column 'CreatedDate'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'CreatedDate'.
    /// </value>
    public DateTime CreatedDate { get; set; }
    
    /// <summary>
    ///     Gets or sets the property value representing column 'FolderId'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'FolderId'.
    /// </value>
    public int? FolderId { get; set; }
    
    /// <summary>
    ///     Gets or sets the property value representing column 'OwnerId'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'OwnerId'.
    /// </value>
    public int OwnerId { get; set; }
    
    #endregion
    
    #region Generated Relationships
    
    /// <summary>
    ///     Gets or sets the navigation property for entity <see cref="ApplicationUser" />.
    /// </summary>
    /// <value>
    ///     The navigation property for entity <see cref="ApplicationUser" />.
    /// </value>
    /// <seealso cref="OwnerId" />
    public virtual ApplicationUser OwnerApplicationUser { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the navigation property for entity <see cref="Folder" />.
    /// </summary>
    /// <value>
    ///     The navigation property for entity <see cref="Folder" />.
    /// </value>
    /// <seealso cref="FolderId" />
    public virtual Folder? FileFolder { get; set; }
    
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
