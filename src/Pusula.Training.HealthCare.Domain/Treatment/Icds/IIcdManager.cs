using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Treatment.Icds;

public interface IIcdManager : IRepository<Icd, Guid>
{
    Task<Icd> CreateAsync(
        string codeChapter,
        string codeNumber,
        string detail
    );
    
    Task<Icd> UpdateAsync(
        Guid id,
        string codeChapter,
        string codeNumber,
        string detail
    );
}