using Microsoft.AspNetCore.Mvc;

namespace SupFile.Back.Core.Dto;

public class ApplicationUserDto
{
    [FromForm(Name = "username")] public required string Username { get; set; }
    
    [FromForm(Name = "firstName")] public required string FirstName { get; set; }
    
    [FromForm(Name = "lastName")] public string? LastName { get; set; }
    
    [FromForm(Name = "status")] public required string Status { get; set; }
    
    [FromForm(Name = "theme")] public required string Theme { get; set; }
    
    [FromForm(Name = "language")] public required string Language { get; set; }
    
    [FromForm(Name = "profilePictureId")] public Guid? ProfilePictureId { get; set; }
    
    
}
