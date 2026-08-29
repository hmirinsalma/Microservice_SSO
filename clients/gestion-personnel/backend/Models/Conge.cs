namespace GestionPersonnel.API.Models;

public enum StatutConge
{
    EnAttente,
    ValideChef,
    ValideDirecteur,
    Refuse,
    Annule
}

public class Conge
{
    public int Id { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public string Motif { get; set; } = string.Empty;
    public StatutConge Statut { get; set; } = StatutConge.EnAttente;

    // Commentaires de validation
    public string? CommentaireChef { get; set; }
    public string? CommentaireDirecteur { get; set; }

    // Dates de traitement
    public DateTime? DateTraitementChef { get; set; }
    public DateTime? DateTraitementDirecteur { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FK Employé demandeur
    public int EmployeId { get; set; }
    public Employe Employe { get; set; } = null!;

    // FK Chef de service qui a traité
    public int? ChefServiceId { get; set; }
    public Employe? ChefService { get; set; }

    // FK Directeur qui a traité
    public int? DirecteurId { get; set; }
    public Employe? Directeur { get; set; }
}
