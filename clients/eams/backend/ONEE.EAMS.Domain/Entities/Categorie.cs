namespace ONEE.EAMS.Domain.Entities;

public class Categorie
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icone { get; set; } = string.Empty;
    public string Couleur { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public ICollection<Equipement> Equipements { get; set; } = new List<Equipement>();
}
