namespace SupFile.Back.Api.Validators;

public class UserPatchModelValidator : AbstractValidator<UserPatchModel>
{
    public UserPatchModelValidator()
    {
        const int MinNameLength = 4;
        const int MaxNameLength = 25;
        RuleFor(x => x.UserName)
            .MinimumLength(MinNameLength)
            .WithMessage(string.Format(CultureInfo.CurrentCulture,
                "Username must be at least {0} characters", MinNameLength))
            .MaximumLength(MaxNameLength)
            .WithMessage(string.Format(CultureInfo.CurrentCulture,
                "Username cannot exceed {0} characters", MaxNameLength))
            .When(x => x.UserName is not null);
    }
}
