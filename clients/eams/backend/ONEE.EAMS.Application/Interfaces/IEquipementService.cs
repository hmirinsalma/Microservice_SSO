using System.Security.Claims;
using ONEE.EAMS.Application.Common;
using ONEE.EAMS.Application.DTOs.Equipement;
using ONEE.EAMS.Application.DTOs.Historique;
using Microsoft.AspNetCore.Http;

namespace ONEE.EAMS.Application.Interfaces;

public interface IEquipementService
{
    Task<PagedResult<EquipementListDto>> GetAllAsync(EquipementFilterRequest filter, ClaimsPrincipal user);
    Task<EquipementDetailDto> GetByIdAsync(Guid id, ClaimsPrincipal user);
    Task<EquipementDetailDto> CreateAsync(CreateEquipementRequest request, ClaimsPrincipal user);
    Task<EquipementDetailDto> UpdateAsync(Guid id, UpdateEquipementRequest request, ClaimsPrincipal user);
    Task DeleteAsync(Guid id, ClaimsPrincipal user);
    Task<EquipementDetailDto> UpdateEtatAsync(Guid id, UpdateEtatRequest request, ClaimsPrincipal user);
    Task<DocumentDto> UploadDocumentAsync(Guid id, IFormFile file, ClaimsPrincipal user);
    Task<PhotoDto> UploadPhotoAsync(Guid id, IFormFile file, ClaimsPrincipal user);
    Task DeleteDocumentAsync(Guid equipementId, Guid documentId, ClaimsPrincipal user);
    Task<PagedResult<HistoriqueEntryDto>> GetHistoriqueAsync(Guid id, int page, int pageSize, ClaimsPrincipal user);
}
