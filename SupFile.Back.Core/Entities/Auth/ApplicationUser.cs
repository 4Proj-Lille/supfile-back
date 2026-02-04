using Microsoft.AspNetCore.Identity;

namespace SupFile.Back.Core.Entities.Auth;

public class ApplicationUser : IdentityUser<int>, IEntity<ApplicationUser, int>
{
    public ApplicationUser()
    {
    }

    // public Guid Id
    // {
    //     get => base.Id;
    //     set => base.Id = value;
    // }

    /// <summary>
    ///     Gets or sets the property value representing column 'RefreshToken'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'RefreshToken'.
    /// </value>
    public string? RefreshToken { get; set; }

    /// <summary>
    ///     Gets or sets the property value representing column 'RefreshTokenExpiresAtUtc'.
    /// </summary>
    /// <value>
    ///     The property value representing column 'RefreshTokenExpiresAtUtc'.
    /// </value>
    public DateTime? RefreshTokenExpiresAtUtc { get; set; }
    
    public UserLanguage Language {get; set;}
    
    public string DisplayName { get; set; }
    
    
    #region Generated Relationships

    
    /// <summary>
    ///     Gets or sets the navigation collection for entity <see cref="Directory" />.
    /// </summary>
    /// <value>
    ///     The navigation collection for entity <see cref="Directory" />.
    /// </value>
    public virtual ICollection<Folder> OwnerFolders { get; }    
    
    /// <summary>
    ///     Gets or sets the navigation collection for entity <see cref="File" />.
    /// </summary>
    /// <value>
    ///     The navigation collection for entity <see cref="File" />.
    /// </value>
    public virtual ICollection<Media> OwnerMedias { get; } 
    
    /// <summary>
    ///     Gets or sets the navigation collection for entity <see cref="Directory" />.
    /// </summary>
    /// <value>
    ///     The navigation collection for entity <see cref="Directory" />.
    /// </value>
    public virtual ICollection<Share> OwnerShares { get; }

    #endregion

}
