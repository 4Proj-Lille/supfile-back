namespace SupFile.Back.Core.Entities;

/// <summary>
///     Entity class representing data for table 'Share'.
/// </summary>
public class Share : BaseEntity<Share, int>, IEntity<Share, int>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Share" /> class.
    /// </summary>
    public Share()
    {
    }

    #region Generated Properties

    /// <summary>
    ///     Gets or sets the property value representing column 'Token'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'Token'.
    /// </value>
    public string Permission { get; set; } = null!;
    
    /// <summary>
    ///     Gets or sets the property value representing column 'ExpirationDate'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'ExpirationDate'.
    /// </value>
    public int UserId { get; set; }
    
    /// <summary>
    ///     Gets or sets the property value representing column Type
    /// </summary>
    /// <value>
    ///     The property value representing column 'Type'.
    /// </value>
    public string Type { get; set; } = null!;
    
    /// <summary>
    ///     Gets or sets the property value representing column 'ShareFileId'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'ShareFileId'.
    /// </value>
    public int? ShareFileId { get; set; }
    
    /// <summary>
    ///     Gets or sets the property value representing column 'ShareDirectoryId'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'ShareDirectoryId'.
    /// </value>
    public int? ShareDirectoryId { get; set; }
    
    #endregion
    
    #region Generated Relationships
    
    /// <summary>
    ///     Gets or sets the navigation property for entity <see cref="ApplicationUser" />.
    /// </summary>
    /// <value>
    ///     The navigation property for entity <see cref="ApplicationUser" />.
    /// </value>
    /// <seealso cref="UserId" />
    public virtual ApplicationUser ShareUser { get; set; } = null!;
    
    /// <summary>
    ///     Gets or sets the navigation property for entity <see cref="File" />.
    /// </summary>
    /// <value>
    ///     The navigation property for entity <see cref="File" />.
    /// </value>
    /// <seealso cref="ShareFileId" />
    public virtual File? ShareFile { get; set; }
    
    /// <summary>
    ///     Gets or sets the navigation property for entity <see cref="Directory" />.
    /// </summary>
    /// <value>
    ///     The navigation property for entity <see cref="Directory" />.
    /// </value>
    /// <seealso cref="ShareDirectoryId" />
    public virtual Directory? ShareDirectory { get; set; }
    
    #endregion
}
