using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Treatment.Icds;

public class Icd : FullAuditedAggregateRoot<Guid>
{
    
    [NotNull]
    public virtual string CodeNumber { get; private set; } = null!;
    
    [NotNull]
    public virtual string Detail { get; private set; } = null!;

    protected Icd()
    {
        CodeNumber = string.Empty;
        Detail = string.Empty;
    }

    public Icd(Guid id, string codeNumber, string detail)
    {
        Id = id;
        SetCodeNumber(codeNumber);
        SetDetail(detail);
    }

    public void SetCodeNumber(string codeNumber)
    {
        Check.NotNullOrWhiteSpace(codeNumber, nameof(codeNumber), IcdConsts.CodeNumberMaxLength, IcdConsts.CodeNumberMinLength);
        CodeNumber = codeNumber.ToUpper();
    }

    public void SetDetail(string detail)
    {
        Check.NotNullOrWhiteSpace(detail, nameof(detail), IcdConsts.DetailMaxLength, IcdConsts.DetailMinLength);
        Detail = detail;
    }
}