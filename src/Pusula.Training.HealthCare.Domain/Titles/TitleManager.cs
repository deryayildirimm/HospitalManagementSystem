using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Titles;

public class TitleManager(ITitleRepository titleRepository) : DomainService
{
    public virtual async Task<Title> CreateAsync(string titleName)
    {
        Check.NotNullOrWhiteSpace(titleName, nameof(titleName), TitleConsts.TitleNameMaxLength, TitleConsts.TitleNameMinLength);
        var title = new Title(
            Guid.NewGuid(),
            titleName
        );
        return await titleRepository.InsertAsync(title);
    }

    public virtual async Task<Title> UpdateAsync(
        Guid id,
        string titleName
    )
    {
        Check.NotNullOrWhiteSpace(titleName, nameof(titleName), TitleConsts.TitleNameMaxLength, TitleConsts.TitleNameMinLength);
        
        var title = await titleRepository.GetAsync(id);
        title.TitleName = titleName;
        
        return await titleRepository.UpdateAsync(title);
    }
}