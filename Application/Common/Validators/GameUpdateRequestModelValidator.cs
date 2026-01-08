using Application.Common.Models;
using FluentValidation;

public class GameUpdateRequestModelValidator : AbstractValidator<GameUpdateRequestModel>
{
    public GameUpdateRequestModelValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().NotNull();
    }
}