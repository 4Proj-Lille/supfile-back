namespace SupFile.Back.Api.Validators;

public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
{
    public ForgotPasswordDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email required").EmailAddress().WithMessage("Invalid email format");
    }
}
