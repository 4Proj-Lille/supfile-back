using Microsoft.AspNetCore.Mvc;

namespace SupFile.Back.Core.Dto;

public class ApplicationUserDto
{
    [FromForm(Name = "username")] public string Username { get; set; }
    [FromForm(Name = "displayName")] public string DisplayName { get; set; }
    [FromForm(Name = "status")] public string Status { get; set; }
    [FromForm(Name = "theme")] public string Theme { get; set; }
    [FromForm(Name = "language")] public string Language { get; set; }
    [FromForm(Name = "profilePictureId")] public Guid? ProfilePictureId { get; set; }
}
