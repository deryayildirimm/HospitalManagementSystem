using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Treatment.Icds;

public class IcdManager(IIcdRepository icdRepository) : DomainService
{
    public virtual async Task<Icd> CreateAsync(string codeChapter, string codeNumber, string detail)
    {
        Check.NotNullOrWhiteSpace(codeChapter, nameof(codeChapter), IcdConsts.CodeChapterLength, IcdConsts.CodeChapterLength);
        Check.NotNullOrWhiteSpace(codeNumber, nameof(codeNumber), IcdConsts.CodeNumberMaxLength, IcdConsts.CodeNumberMinLength);
        Check.NotNullOrWhiteSpace(detail, nameof(detail), IcdConsts.DetailMaxLength, IcdConsts.DetailMinLength);
        
        var icd = new Icd(
            Guid.NewGuid(),
            codeChapter,
            codeNumber,
            detail
        );
        return await icdRepository.InsertAsync(icd);
    }

    public virtual async Task<Icd> UpdateAsync(
        Guid id,
        string codeChapter,
        string codeNumber,
        string detail
    )
    {
        Check.NotNullOrWhiteSpace(codeChapter, nameof(codeChapter), IcdConsts.CodeChapterLength, IcdConsts.CodeChapterLength);
        Check.NotNullOrWhiteSpace(codeNumber, nameof(codeNumber), IcdConsts.CodeNumberMaxLength, IcdConsts.CodeNumberMinLength);
        Check.NotNullOrWhiteSpace(detail, nameof(detail), IcdConsts.DetailMaxLength, IcdConsts.DetailMinLength);

        var icd = await icdRepository.GetAsync(id);
        
        icd.SetCodeChapter(codeChapter);
        icd.SetCodeNumber(codeNumber);
        icd.SetDetail(detail);
        
        return await icdRepository.UpdateAsync(icd);
    }
}