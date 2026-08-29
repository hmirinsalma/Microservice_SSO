using ONEE.EAMS.Domain.Enums;

namespace ONEE.EAMS.Application.DTOs.Equipement;

public record EquipementListDto(
    Guid Id, string Reference, string Nom, string CategorieName, string CategorieCode, string CouleurCategorie,
    string Type, string Marque, string Modele, string NumeroSerie, string Localisation,
    string ServiceNom, string ResponsableNom, DateTime DateInstallation, EquipementEtat Etat,
    DateTime? DateFinGarantie, decimal? ValeurAcquisition);

public record EquipementDetailDto(
    Guid Id, string Reference, string Nom,
    Guid CategorieId, string CategorieName, string CategorieCode, string CouleurCategorie, string IconeCategorie,
    string Type, string Marque, string Modele, string NumeroSerie, string Localisation,
    Guid ServiceId, string ServiceNom, Guid ResponsableId, string ResponsableNom,
    DateTime DateInstallation, DateTime? DateMiseEnService, EquipementEtat Etat,
    DateTime? DateFinGarantie, decimal? ValeurAcquisition, string? Fournisseur, string? Description,
    DateTime CreatedAt, DateTime UpdatedAt,
    IEnumerable<DocumentDto> Documents, IEnumerable<PhotoDto> Photos);

public record DocumentDto(Guid Id, string NomFichier, string Url, string Extension, long TailleOctets, DateTime UploadedAt);
public record PhotoDto(Guid Id, string Url, bool IsMain, DateTime UploadedAt);

public record CreateEquipementRequest(
    string Nom, Guid CategorieId, string Type, string Marque, string Modele, string NumeroSerie,
    string Localisation, Guid ServiceId, Guid ResponsableId, DateTime DateInstallation,
    DateTime? DateMiseEnService, EquipementEtat Etat, DateTime? DateFinGarantie,
    decimal? ValeurAcquisition, string? Fournisseur, string? Description);

public record UpdateEquipementRequest(
    string Nom, Guid CategorieId, string Type, string Marque, string Modele,
    string Localisation, Guid ServiceId, Guid ResponsableId, DateTime DateInstallation,
    DateTime? DateMiseEnService, EquipementEtat Etat, DateTime? DateFinGarantie,
    decimal? ValeurAcquisition, string? Fournisseur, string? Description);

public record UpdateEtatRequest(EquipementEtat Etat);

public record EquipementFilterRequest
{
    public string? Search { get; init; }
    public Guid? CategorieId { get; init; }
    public string? Type { get; init; }
    public EquipementEtat? Etat { get; init; }
    public Guid? ServiceId { get; init; }
    public Guid? ResponsableId { get; init; }
    public Guid? TechnicienId { get; init; }
    public string? Localisation { get; init; }
    public DateTime? DateInstallationFrom { get; init; }
    public DateTime? DateInstallationTo { get; init; }
    public string? SortBy { get; init; }
    public bool SortDesc { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
