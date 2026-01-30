namespace SupFile.Back.Api.Validators;

public class ResendVerificationEmailDtoValidator : AbstractValidator<ResendVerificationEmailDto>
{
    public ResendVerificationEmailDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email required").EmailAddress().WithMessage("Invalid email");
    }
}
