namespace ONEE.EAMS.Domain.Entities;

public class ServiceEntity
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Equipement> Equipements { get; set; } = new List<Equipement>();
}
