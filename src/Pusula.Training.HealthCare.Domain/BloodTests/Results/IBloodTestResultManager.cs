using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.BloodTests.Results
{
    public interface IBloodTestResultManager : IDomainService
    {
        Task<BloodTestResult> CreateAsync(Guid testId, double value);
        Task<BloodTestResult> UpdateAsync(Guid id, Guid testId, double value);
    }
}
