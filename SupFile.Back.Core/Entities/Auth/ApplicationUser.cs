using Microsoft.AspNetCore.Identity;

namespace SupFile.Back.Core.Entities.Auth;

public class ApplicationUser : IdentityUser<int>, IEntity<ApplicationUser, int>
{
    public ApplicationUser()
    {
    }

    public string DisplayName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAtUtc { get; set; }
    public UserLanguage Language { get; set; }


    #region Generated Relationships

    /// <summary>
    ///     Gets or sets the navigation collection for entity <see cref="Directory" />.
    /// </summary>
    /// <value>
    ///     The navigation collection for entity <see cref="Directory" />.
    /// </value>
    public virtual ICollection<Directory> OwnedDirectories { get; }

    /// <summary>
    ///     Gets or sets the navigation collection for entity <see cref="File" />.
    /// </summary>
    /// <value>
    ///     The navigation collection for entity <see cref="File" />.
    /// </value>
    public virtual ICollection<File> OwnedFiles { get; }

    /// <summary>
    ///     Gets or sets the navigation collection for entity <see cref="Directory" />.
    /// </summary>
    /// <value>
    ///     The navigation collection for entity <see cref="Directory" />.
    /// </value>
    public virtual ICollection<Share> OwnedShares { get; }

    #endregion
}
