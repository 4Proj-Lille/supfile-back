namespace SupFile.Back.Core.Dto;

public class ConfirmEmailDto
{
    [Required] public required string Code { get; set; }
}
