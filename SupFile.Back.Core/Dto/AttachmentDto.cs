using Microsoft.AspNetCore.Mvc;

namespace SupFile.Back.Core.Dto;

public class AttachmentDto
{
    [FromForm(Name = "id")] public required Guid Id { get; set; }

    [FromForm(Name = "name")] public required string? Name { get; set; }

    [FromForm(Name = "type")] public required string Type { get; set; }

    [FromForm(Name = "ownerid")] public required int OwnerId { get; set; }
}
