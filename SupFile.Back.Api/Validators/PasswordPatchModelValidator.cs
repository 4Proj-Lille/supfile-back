using System.Text.RegularExpressions;

namespace SupFile.Back.Api.Validators;

public partial class PasswordPatchModelValidator : AbstractValidator<PasswordPatchModel>
{
    public PasswordPatchModelValidator()
    {
        const int MinNameLength = 8;
        RuleFor(vm => vm.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required");
        RuleFor(vm => vm.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required")
            .MinimumLength(MinNameLength)
            .WithMessage(string.Format(CultureInfo.CurrentCulture,
                "Password must be at least {0} characters", MinNameLength))
            .Matches(UpperCaseRegex())
            .WithMessage("Password must contain at least one uppercase letter")
            .Matches(SpecialCharRegex())
            .WithMessage("Password must contain at least one special character");
        RuleFor(vm => vm.ConfirmNewPassword)
            .NotEmpty()
            .WithMessage("Confirm New Password is required")
            .Equal(x => x.NewPassword)
            .WithMessage("Confirm new password must match new password");
    }

    [GeneratedRegex(@"[A-Z]")]
    private static partial Regex UpperCaseRegex();

    [GeneratedRegex(@"[^\da-zA-Z]")]
    private static partial Regex SpecialCharRegex();
}
