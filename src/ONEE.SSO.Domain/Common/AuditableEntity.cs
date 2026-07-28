namespace ONEE.SSO.Domain.Common;

public abstract class AuditableEntity : BaseAuditableEntity
{
    public Guid? CreatedBy { get; set; }

    public Guid? UpdatedBy { get; set; }
}