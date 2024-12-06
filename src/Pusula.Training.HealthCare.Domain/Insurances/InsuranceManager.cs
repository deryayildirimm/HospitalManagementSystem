using Pusula.Training.HealthCare.Insurances;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Insurances
{
    public class InsuranceManager(IInsuranceRepository insuranceRepository) : DomainService, IInsuranceManager
    {
        public async Task<Insurance> CreateAsync(
            string policyNumber, 
            EnumInsuranceCompanyName insuranceCompanyName, 
            decimal? premiumAmount = null, 
            decimal? coverageAmount = null, 
            DateTime? startDate = null, 
            DateTime? endDate = null, 
            string? description = null)
        {
            Check.NotNullOrWhiteSpace(policyNumber, nameof(policyNumber), InsuranceConst.MaxPolicyNumberLength, InsuranceConst.MinPolicyNumberLength);
            Check.Range((int)insuranceCompanyName, nameof(insuranceCompanyName), InsuranceConst.MinInsuranceCompanyName, InsuranceConst.MaxInsuranceCompanyName);

            var Insurance = new Insurance(GuidGenerator.Create(), policyNumber, insuranceCompanyName, premiumAmount, coverageAmount, startDate, endDate, description);
            return await insuranceRepository.InsertAsync(Insurance);
        }

        public async Task<Insurance> UpdateAsync(
            Guid id,
            string policyNumber,
            EnumInsuranceCompanyName insuranceCompanyName,
            decimal? premiumAmount = null,
            decimal? coverageAmount = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? description = null)
        {
            Check.NotNullOrWhiteSpace(policyNumber, nameof(policyNumber), InsuranceConst.MaxPolicyNumberLength, InsuranceConst.MinPolicyNumberLength);
            Check.Range((int)insuranceCompanyName, nameof(insuranceCompanyName), InsuranceConst.MinInsuranceCompanyName, InsuranceConst.MaxInsuranceCompanyName);

            var insurance = await insuranceRepository.GetAsync(id);

            insurance.SetPolicyNumber(policyNumber);
            insurance.SetInsuranceCompanyName(insuranceCompanyName);
            insurance.SetPremiumAmount(premiumAmount);
            insurance.SetCoverageAmount(coverageAmount);
            insurance.SetStartDate(startDate);
            insurance.SetEndDate(endDate);
            insurance.SetDescription(description);

            return await insuranceRepository.UpdateAsync(insurance);
        }

    }
}
