using FluentValidation;
using GestionPersonnel.API.DTOs.Direction;

namespace GestionPersonnel.API.Validators;

public class CreateDirectionValidator : AbstractValidator<CreateDirectionDto>
{
    public CreateDirectionValidator()
    {
        RuleFor(x => x.Nom).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description != null);
    }
}
