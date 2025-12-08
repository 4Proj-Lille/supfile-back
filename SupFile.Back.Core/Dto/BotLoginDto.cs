using Microsoft.AspNetCore.Mvc;

namespace SupFile.Back.Core.Dto;

public class BotLoginDto
{
    [FromForm(Name = "ClientId")] public required Guid ClientId { get; set; }

    [FromForm(Name = "ClientSecret")] public required string ClientSecret { get; set; }
}
