using TIMS.API.Common;
using TIMS.API.DTOs.Intervention;

namespace TIMS.API.Interfaces;

public interface IInterventionService
{
    Task<PagedResult<InterventionListDto>> GetAllAsync(InterventionFilterDto filter, int userId, string role, int? serviceId);
    Task<InterventionDto> GetByIdAsync(int id, int userId, string role, int? serviceId);
    Task<InterventionDto> CreateAsync(CreateInterventionDto dto, int userId, string role, int? serviceId);
    Task<InterventionDto> UpdateAsync(int id, UpdateInterventionDto dto, int userId, string role, int? serviceId);
    Task DeleteAsync(int id, int userId);
    Task<InterventionDto> ChangeStatusAsync(int id, ChangeStatusDto dto, int userId, string role);
    Task<InterventionDto> ChangePriorityAsync(int id, ChangePriorityDto dto, int userId, string role);
    Task<InterventionDto> AssignTechnicienAsync(int id, AssignTechnicienDto dto, int userId, string role, int? serviceId);
    Task<CommentDto> AddCommentAsync(int id, AddCommentDto dto, int userId, string role);
    Task<InterventionDto> UpdateCompteRenduAsync(int id, UpdateCompteRenduDto dto, int userId);
    Task<List<HistoryDto>> GetHistoryAsync(int id, int userId, string role, int? serviceId);
    Task<AttachmentDto> AddAttachmentAsync(int id, IFormFile file, int userId, string role);
    Task DeleteAttachmentAsync(int attachmentId, int userId);
    Task<string> GetAttachmentUrlAsync(int attachmentId, int userId, string role, int? serviceId);
}
