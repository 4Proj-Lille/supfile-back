namespace SupFile.Back.Core.Entities;

/// <summary>
///     Entity class representing data for table 'User'.
/// </summary>
public class ApplicationUser : BaseEntity<ApplicationUser, int>, IEntity<ApplicationUser, int>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Entities.ApplicationUser" /> class.
    /// </summary>
    public ApplicationUser()
    {
    }

    #region Generated Relationships

    /// <summary>
    ///     Gets or sets the navigation property for entity <see cref="IdentityUser" />.
    /// </summary>
    /// <value>
    ///     The navigation property for entity <see cref="IdentityUser" />.
    /// </value>
    /// <seealso cref="IdentityUserId" />
    public virtual AuthIdentityUser? IdentityUser { get; set; }

    #endregion

    #region Generated Properties

    /// <summary>
    ///     Gets or sets the property value representing column 'IdentityUserId'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'IdentityUserId'.
    /// </value>
    public Guid? IdentityUserId { get; set; }

    /// <summary>
    ///     Gets or sets the property value representing column 'Username'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'Username'.
    /// </value>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the property value representing column 'Firstname'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'Firstname'.
    /// </value>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the property value representing column 'Lastname'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'Lastname'.
    /// </value>
    public string? LastName { get; set; }

    /// <summary>
    ///     Gets or sets the property value representing column 'Language'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'Language'.
    /// </value>
    public UserLanguage Language { get; set; }

    /// <summary>
    ///     Gets or sets the property value representing column 'ProfilePictureId'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'ProfilePictureId'.
    /// </value>
    public Guid? ProfilePictureId { get; set; }

    #endregion
}
