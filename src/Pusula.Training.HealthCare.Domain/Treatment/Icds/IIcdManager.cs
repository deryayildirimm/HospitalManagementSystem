using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Treatment.Icds;

public interface IIcdManager
{
    Task<Icd> CreateAsync(
        string codeNumber,
        string detail
    );
    
    Task<Icd> UpdateAsync(
        Guid id,
        string codeNumber,
        string detail
    );
}