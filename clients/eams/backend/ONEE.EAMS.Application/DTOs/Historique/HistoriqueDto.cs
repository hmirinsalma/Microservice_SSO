namespace ONEE.EAMS.Application.DTOs.Historique;

public record HistoriqueEntryDto(Guid Id, string EntiteType, string TypeEvenement, string? ValeurAvant, string? ValeurApres, string AuteurNom, DateTime HorodatageUtc);
