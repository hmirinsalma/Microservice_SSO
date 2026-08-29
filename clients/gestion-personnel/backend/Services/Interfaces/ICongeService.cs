using GestionPersonnel.API.DTOs.Conge;
using GestionPersonnel.API.DTOs.Common;

namespace GestionPersonnel.API.Services.Interfaces;

public interface ICongeService
{
    // Employé — créer sa propre demande
    Task<CongeDto> CreateAsync(int employeId, CreateCongeDto dto);

    // Employé — voir ses propres congés
    Task<IEnumerable<CongeDto>> GetMyCongesAsync(int employeId);

    // Chef de service — voir les congés de son service
    Task<IEnumerable<CongeDto>> GetByServiceAsync(int serviceId, string? statut = null);

    // Directeur — voir les congés de sa direction (validés par chef)
    Task<IEnumerable<CongeDto>> GetByDirectionAsync(int directionId, string? statut = null);

    // Admin RH — voir tous les congés avec filtres
    Task<PagedResultDto<CongeDto>> GetAllAsync(CongeQueryDto query);

    // Chef de service — valider/refuser
    Task<CongeDto> TraiterParChefAsync(int congeId, int chefEmployeId, TraiterCongeDto dto);

    // Directeur — validation finale
    Task<CongeDto> TraiterParDirecteurAsync(int congeId, int directeurEmployeId, TraiterCongeDto dto);

    // Annuler (employé lui-même, si EnAttente)
    Task AnnulerAsync(int congeId, int employeId);

    Task<CongeDto> GetByIdAsync(int id);
}
