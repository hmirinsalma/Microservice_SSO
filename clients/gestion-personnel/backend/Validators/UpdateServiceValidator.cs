using FluentValidation;
using GestionPersonnel.API.DTOs.Service;

namespace GestionPersonnel.API.Validators;

public class UpdateServiceValidator : AbstractValidator<UpdateServiceDto>
{
    public UpdateServiceValidator()
    {
        RuleFor(x => x.Nom).NotEmpty().WithMessage("Le nom est requis.").MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description != null);
        RuleFor(x => x.DirectionId).GreaterThan(0).WithMessage("La direction est requise.");
    }
}
