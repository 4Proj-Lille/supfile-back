using Microsoft.AspNetCore.Mvc;

namespace SupFile.Back.Core.Dto;

public class ChannelDto
{
    [FromForm(Name = "name")] public required string Name { get; set; }
    
    [FromForm(Name = "visibility")] public required string Visibility { get; set; }
    
    [FromForm(Name = "workspaceId")] public required int WorkspaceId { get; set; }
    
}
