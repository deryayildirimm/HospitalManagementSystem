using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Treatment.Icds;

public class Icd : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public virtual string CodeChapter { get; protected set; }
    
    [NotNull]
    public virtual string CodeNumber { get; protected set; }
    
    [NotNull]
    public virtual string Detail { get; protected set; }

    protected Icd()
    {
        CodeChapter = string.Empty;
        CodeNumber = string.Empty;
        Detail = string.Empty;
    }

    public Icd(Guid id, string codeChapter, string codeNumber, string detail)
    {
        Id = id;
        SetCodeChapter(codeChapter);
        SetCodeNumber(codeNumber);
        SetDetail(detail);
    }

    public void SetCodeChapter(string codeChapter)
    {
        Check.NotNullOrWhiteSpace(codeChapter, nameof(codeChapter), IcdConsts.CodeChapterLength, IcdConsts.CodeChapterLength);
        CodeChapter = codeChapter.ToUpper();
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