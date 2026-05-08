using System.Text.RegularExpressions;

namespace SupFile.Back.Api.Validators;

public partial class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator(IStringLocalizer<AuthenticationsRes> l, IStringLocalizer<SharedRes> sl)
    {
        const int MaxNameLength = 25;
        const int MinNameLength = 4;
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MinimumLength(MinNameLength)
            .WithMessage(string.Format(CultureInfo.CurrentCulture,
                "Username must be at least {0} characters", MinNameLength))
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
            .Matches(UpperCaseRegex())
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches(LowerCaseRegex())
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches(DigitRegex())
            .WithMessage("Password must contain at least one number.")
            .Matches(SpecialCharRegex())
            .WithMessage("Password must contain at least one special character.")
            .Matches(NoWhitespaceRegex())
            .WithMessage("Password must not contain whitespace.");
    }

    [GeneratedRegex(@"[A-Z]")]
    private static partial Regex UpperCaseRegex();

    [GeneratedRegex(@"[a-z]")]
    private static partial Regex LowerCaseRegex();

    [GeneratedRegex(@"\d")]
    private static partial Regex DigitRegex();

    [GeneratedRegex(@"[^\da-zA-Z]")]
    private static partial Regex SpecialCharRegex();

    [GeneratedRegex(@"^\S+$")]
    private static partial Regex NoWhitespaceRegex();
}
