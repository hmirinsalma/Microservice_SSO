using FluentValidation;
using GestionPersonnel.API.DTOs.User;

namespace GestionPersonnel.API.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3).MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.RoleId).GreaterThan(0).WithMessage("Un rôle est requis.");
    }
}
