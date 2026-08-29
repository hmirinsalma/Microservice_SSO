namespace GestionPersonnel.API.Models;

public class Role
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
}
