using TIMS.API.Entities;

namespace TIMS.API.DTOs.Intervention;

public class InterventionDto
{
    public int Id { get; set; }
    public string NumeroIntervention { get; set; } = string.Empty;
    public string Objet { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TypeIntervention { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string Localisation { get; set; } = string.Empty;
    public string Equipement { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime DatePrevue { get; set; }
    public DateTime? DateCloture { get; set; }
    public InterventionPriority Priority { get; set; }
    public string PriorityLabel { get; set; } = string.Empty;
    public InterventionStatus Status { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public string? CompteRendu { get; set; }
    public UserRefDto? Responsable { get; set; }
    public UserRefDto? ChefService { get; set; }
    public UserRefDto? Technicien { get; set; }
    public RefDto? Equipe { get; set; }
    public RefDto? Service { get; set; }
    public UserRefDto? CreatedBy { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
    public List<AttachmentDto> Attachments { get; set; } = new();
}

public class InterventionListDto
{
    public int Id { get; set; }
    public string NumeroIntervention { get; set; } = string.Empty;
    public string Objet { get; set; } = string.Empty;
    public string TypeIntervention { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime DatePrevue { get; set; }
    public DateTime? DateCloture { get; set; }
    public InterventionPriority Priority { get; set; }
    public string PriorityLabel { get; set; } = string.Empty;
    public InterventionStatus Status { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public UserRefDto? Responsable { get; set; }
    public UserRefDto? Technicien { get; set; }
    public RefDto? Equipe { get; set; }
    public RefDto? Service { get; set; }
}

public class CreateInterventionDto
{
    public string Objet { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TypeIntervention { get; set; } = string.Empty;
    public string Categorie { get; set; } = string.Empty;
    public string Localisation { get; set; } = string.Empty;
    public string Equipement { get; set; } = string.Empty;
    public DateTime DatePrevue { get; set; }
    public InterventionPriority Priority { get; set; } = InterventionPriority.Normale;
    public int? ResponsableId { get; set; }
    public int? TechnicienId { get; set; }
    public int? EquipeId { get; set; }
    public int? ServiceId { get; set; }
}

public class UpdateInterventionDto
{
    public string? Objet { get; set; }
    public string? Description { get; set; }
    public string? TypeIntervention { get; set; }
    public string? Categorie { get; set; }
    public string? Localisation { get; set; }
    public string? Equipement { get; set; }
    public DateTime? DatePrevue { get; set; }
    public InterventionPriority? Priority { get; set; }
    public int? ResponsableId { get; set; }
    public int? TechnicienId { get; set; }
    public int? EquipeId { get; set; }
}

public class ChangeStatusDto
{
    public InterventionStatus NewStatus { get; set; }
}

public class ChangePriorityDto
{
    public InterventionPriority NewPriority { get; set; }
}

public class AssignTechnicienDto
{
    public int? TechnicienId { get; set; }
}

public class AddCommentDto
{
    public string Content { get; set; } = string.Empty;
}

public class UpdateCompteRenduDto
{
    public string CompteRendu { get; set; } = string.Empty;
}

public class CommentDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public UserRefDto? Author { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AttachmentDto
{
    public int Id { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserRefDto? UploadedBy { get; set; }
}

public class HistoryDto
{
    public int Id { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string? FieldChanged { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Description { get; set; }
    public UserRefDto? Author { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UserRefDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePhotoPath { get; set; }
}

public class RefDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class InterventionFilterDto
{
    public string? NumeroIntervention { get; set; }
    public string? Objet { get; set; }
    public int? TechnicienId { get; set; }
    public int? ResponsableId { get; set; }
    public int? EquipeId { get; set; }
    public InterventionPriority? Priority { get; set; }
    public InterventionStatus? Status { get; set; }
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public int? ServiceId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortOrder { get; set; } = "desc";
}
