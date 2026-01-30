namespace SupFile.Back.Core.Dto;

public class ResetPasswordDto
{
    public required int UserId { get; set; }
    public required string Token { get; set; }
    public required string NewPassword { get; set; }
}
