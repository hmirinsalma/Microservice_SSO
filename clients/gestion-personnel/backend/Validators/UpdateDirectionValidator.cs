using FluentValidation;
using GestionPersonnel.API.DTOs.Direction;

namespace GestionPersonnel.API.Validators;

public class UpdateDirectionValidator : AbstractValidator<UpdateDirectionDto>
{
    public UpdateDirectionValidator()
    {
        RuleFor(x => x.Nom).NotEmpty().WithMessage("Le nom est requis.").MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description != null);
    }
}
