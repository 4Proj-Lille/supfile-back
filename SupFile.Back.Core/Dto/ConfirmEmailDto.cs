namespace SupFile.Back.Core.Dto;

public class ConfirmEmailDto
{
    [Required] public required int UserId { get; set; }
    [Required] public required string Code { get; set; }
}
