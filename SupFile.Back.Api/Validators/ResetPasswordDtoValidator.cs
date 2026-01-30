namespace SupFile.Back.Api.Validators;

public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordDtoValidator()
    {
        RuleFor(x => x.Token).NotEmpty().WithMessage("Code required");
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage("new password required");
    }
}
