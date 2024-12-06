using Pusula.Training.HealthCare.Insurances;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Insurances
{
    public interface IInsuranceManager : IDomainService
    {
        Task<Insurance> CreateAsync(
            string policyNumber,
            EnumInsuranceCompanyName insuranceCompanyName,
            decimal? premiumAmount = null,
            decimal? coverageAmount = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? description = null);

        Task<Insurance> UpdateAsync(
            Guid id,
            string policyNumber,
            EnumInsuranceCompanyName insuranceCompanyName,
            decimal? premiumAmount = null,
            decimal? coverageAmount = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? description = null);
    }
}
