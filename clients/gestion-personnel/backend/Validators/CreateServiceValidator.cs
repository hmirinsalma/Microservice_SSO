using FluentValidation;
using GestionPersonnel.API.DTOs.Service;

namespace GestionPersonnel.API.Validators;

public class CreateServiceValidator : AbstractValidator<CreateServiceDto>
{
    public CreateServiceValidator()
    {
        RuleFor(x => x.Nom).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description != null);
        RuleFor(x => x.DirectionId).GreaterThan(0);
    }
}
