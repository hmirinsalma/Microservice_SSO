using FluentValidation;
using GestionPersonnel.API.DTOs.User;

namespace GestionPersonnel.API.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.RoleId).GreaterThan(0).WithMessage("Un rôle est requis.");
        // Password optionnel — stub temporaire uniquement
        RuleFor(x => x.Password)
            .MinimumLength(6).When(x => !string.IsNullOrWhiteSpace(x.Password))
            .WithMessage("Minimum 6 caractères.");
    }
}
