using Microsoft.AspNetCore.Mvc;

namespace SupFile.Back.Core.Dto;

public class LoginDto
{
    public string Email { get; set; }

    public string Password { get; set; }
}
