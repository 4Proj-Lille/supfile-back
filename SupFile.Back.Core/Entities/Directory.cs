using SupFile.Back.Core.Entities;

namespace SupFile.Back.Core.Entities;

/// <summary>
///     Entity class representing data for table 'Directory'.
/// </summary>
public class Directory : BaseEntity<Directory, int>, IEntity<Directory, int>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Directory" /> class.
    /// </summary>
    public Directory()
    {
        #region Generated Constructor
        
        Files = new HashSet<File>();
        Links = new HashSet<Link>();
        ParentDirectories = new HashSet<Directory>();
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

    #endregion
    
    #region Generated Relationships
    
    /// <summary>
    ///     Gets or sets the navigation property for entity <see cref="ApplicationUser" />.
    /// </summary>
    /// <value>
    ///     The navigation property for entity <see cref="ApplicationUser" />.
    /// </value>
    /// <seealso cref="OwnerId" />
    public virtual ApplicationUser OwnerApplicationUserDirectory { get; set; } = null!;
    
    /// <summary>
    ///     Gets or sets the navigation property for entity <see cref="Directory" />.
    /// </summary>
    /// <value>
    ///     The navigation property for entity <see cref="Directory" />.
    /// </value>
    /// <seealso cref="ParentId" />
    public virtual Directory? ParentDirectory { get; set; }
    
    /// <summary>
    ///     Gets or sets the navigation collection for entity <see cref="Directory" />.
    /// </summary>
    /// <value>
    ///     The navigation collection for entity <see cref="Directory" />.
    /// </value>
    public virtual ICollection<Directory> ParentDirectories { get; }
    
    /// <summary>
    ///     Gets or sets the navigation collection for entity <see cref="File" />.
    /// </summary>
    /// <value>
    ///     The navigation collection for entity <see cref="File" />.
    /// </value>
    public virtual ICollection<File> Files { get; }
    
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
