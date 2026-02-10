namespace SupFile.Back.Api.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator(IStringLocalizer<FoldersRes> localizer)
    {
        const int maxNameLength = 100;
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Name required")
            .MaximumLength(maxNameLength).WithMessage(string.Format(CultureInfo.CurrentCulture,
                "Name cannot exceed {0} characters", maxNameLength));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Name required")
            .MaximumLength(maxNameLength).WithMessage(string.Format(CultureInfo.CurrentCulture,
                "Name cannot exceed {0} characters", maxNameLength));
    }
}
