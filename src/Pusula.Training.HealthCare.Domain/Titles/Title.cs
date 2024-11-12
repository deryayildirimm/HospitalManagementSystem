using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Titles;

public class Title : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public virtual string TitleName { get; set; }

    protected Title()
    {
        TitleName = string.Empty;
    }

    public Title(Guid id, string titleName)
    {
        Check.NotNullOrWhiteSpace(titleName, nameof(titleName), TitleConsts.TitleNameMaxLength, TitleConsts.TitleNameMinLength);

        Id = id;
        TitleName = titleName;
    }
}