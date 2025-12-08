using Microsoft.AspNetCore.Identity;

namespace SupFile.Back.Core.Entities.Auth;

public class AuthIdentityUser : IdentityUser<Guid>, IEntity<AuthIdentityUser, Guid>
{
    public AuthIdentityUser()
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


    #region Generated Relationships

    /// <summary>
    ///     Gets or sets the navigation property for entity <see cref="ApplicationUser" />.
    /// </summary>
    /// <value>
    ///     The navigation property for entity <see cref="ApplicationUser" />.
    /// </value>
    public virtual ApplicationUser? ApplicationUser { get; set; }

    #endregion
}
