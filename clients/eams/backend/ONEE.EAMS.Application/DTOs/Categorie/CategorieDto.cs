namespace ONEE.EAMS.Application.DTOs.Categorie;

public record CategorieDto(Guid Id, string Nom, string Description, string Icone, string Couleur, string Code, int NbEquipements);

public record CreateCategorieRequest(string Nom, string Description, string Icone, string Couleur, string Code);

public record UpdateCategorieRequest(string Nom, string Description, string Icone, string Couleur);
