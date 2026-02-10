namespace Supchat.Back.Api.Validators;

public class FolderPostModelValidator : AbstractValidator<FolderPostModel>
{
    public FolderPostModelValidator(IStringLocalizer<FoldersRes> localizer)
    {
        const int maxNameLength = 100;
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name required")
            .MaximumLength(maxNameLength).WithMessage(string.Format(CultureInfo.CurrentCulture,
                "Name cannot exceed {0} characters", maxNameLength));
    }
}
