using FluentValidation;
using GestionPersonnel.API.DTOs.Employe;

namespace GestionPersonnel.API.Validators;

public class UpdateEmployeValidator : AbstractValidator<UpdateEmployeDto>
{
    public UpdateEmployeValidator()
    {
        RuleFor(x => x.Nom).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Prenom).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Telephone).MaximumLength(20).When(x => x.Telephone != null);
        RuleFor(x => x.DateEmbauche).NotEmpty().LessThanOrEqualTo(DateTime.Today);
        RuleFor(x => x.Poste).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DirectionId).GreaterThan(0);
        RuleFor(x => x.ServiceId).GreaterThan(0);
        RuleFor(x => x.Statut)
            .Must(s => new[] { "Actif", "Inactif", "Suspendu" }.Contains(s))
            .WithMessage("Le statut doit être Actif, Inactif ou Suspendu.");
    }
}
