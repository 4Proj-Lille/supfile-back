namespace SupFile.Back.Api.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator(IStringLocalizer<AuthenticationsRes> l, IStringLocalizer<SharedRes> sl)
    {
        const int MaxNameLength = 25;
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MaximumLength(MaxNameLength)
            .WithMessage(string.Format(CultureInfo.CurrentCulture,
                "Name cannot exceed {0} characters", MaxNameLength));

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email is not valid.");

        const int MaxPasswordLength = 150;
        const int MinPasswordLength = 8;
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MaximumLength(MaxPasswordLength)
            .WithMessage(string.Format(CultureInfo.CurrentCulture, "Password cannot exceed {0} characters",
                MaxPasswordLength))
            .MinimumLength(MinPasswordLength)
            .WithMessage(string.Format(CultureInfo.CurrentCulture, "Password must be at least {0} characters",
                MinPasswordLength))
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")
            .WithMessage(
                "Password must contain at least " +
                "one uppercase letter, " +
                "one lowercase letter, " +
                "one number, " +
                "and one special character.");
    }
}
