using GestionPersonnel.API.Models;

namespace GestionPersonnel.API.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    /// <summary>Utilisé par SsoAuthService pour résoudre un User depuis le claim 'sub'</summary>
    Task<User?> GetBySsoIdAsync(string ssoId);
}
