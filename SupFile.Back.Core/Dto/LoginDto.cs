using Microsoft.AspNetCore.Mvc;

namespace SupFile.Back.Core.Dto;

public class LoginDto
{
    [FromForm(Name = "email")] public required string Email { get; set; }

    [FromForm(Name = "password")] public required string Password { get; set; }
}
