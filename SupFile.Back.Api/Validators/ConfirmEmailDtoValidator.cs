namespace SupFile.Back.Api.Validators;

public class ConfirmEmailDtoValidator : AbstractValidator<ConfirmEmailDto>
{
    public ConfirmEmailDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code required");
    }
}
