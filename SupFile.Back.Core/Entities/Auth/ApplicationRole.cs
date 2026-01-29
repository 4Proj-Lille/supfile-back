using Microsoft.AspNetCore.Identity;

namespace SupFile.Back.Core.Entities.Auth;

public class ApplicationRole : IdentityRole<int>
{
    //public ApplicationRole() { }

    //public ApplicationRole(string roleName)
    //    : base(roleName) { }


    public List<ApplicationUserRole> UserRoles { get; set; }

    public List<ApplicationUser> Users { get; set; }
}
