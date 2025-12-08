using Microsoft.AspNetCore.Mvc;

namespace SupFile.Back.Core.Dto;

public class WorkspaceDto
{
    [FromForm(Name = "name")] public required string Name { get; set; }
    
    [FromForm(Name = "description")] public string? Description { get; set; }
    
    [FromForm(Name = "visibility")] public required string Visibility { get; set; }
    
    [FromForm(Name = "ownerId")] public required int OwnerId { get; set; }
    
    [FromForm(Name = "profilePictureId")] public Guid? ProfilePictureId { get; set; }
}
