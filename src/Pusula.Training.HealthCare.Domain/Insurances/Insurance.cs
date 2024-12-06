using JetBrains.Annotations;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Insurances
{
    public class Insurance : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string PolicyNumber { get; private set; } = string.Empty;
        [CanBeNull]
        public virtual decimal? PremiumAmount { get; private set; } 
        [CanBeNull]
        public virtual decimal? CoverageAmount { get; private set; }
        [CanBeNull]
        public virtual DateTime? StartDate { get; private set; }
        [CanBeNull]
        public virtual DateTime? EndDate { get; private set; }
        [NotNull]
        public virtual EnumInsuranceCompanyName InsuranceCompanyName { get; private set; } = EnumInsuranceCompanyName.None;
        [CanBeNull]
        public virtual string? Description { get; private set; }

        public Insurance(Guid id, string policyNumber, EnumInsuranceCompanyName insuranceCompanyName, decimal? premiumAmount = null, decimal? coverageAmount = null, 
            DateTime? startDate = null, DateTime? endDate = null, string? description = null)
        {
            SetId(id);
            SetPolicyNumber(policyNumber);
            SetPremiumAmount(premiumAmount);
            SetCoverageAmount(coverageAmount);
            SetStartDate(startDate);
            SetEndDate(endDate);
            SetInsuranceCompanyName(insuranceCompanyName);
            SetDescription(description);
        }

        public void SetId(Guid id)
        {
            Id = id;
        }
        public void SetPolicyNumber(string policyNumber)
        {
            Check.NotNullOrWhiteSpace(policyNumber, nameof(policyNumber), InsuranceConst.MaxPolicyNumberLength, InsuranceConst.MinPolicyNumberLength);
            PolicyNumber = policyNumber;
        }
        public void SetPremiumAmount(decimal? premiumAmount)
        {
            PremiumAmount = premiumAmount;
        }
        public void SetCoverageAmount(decimal? coverageAmount)
        {
            CoverageAmount = coverageAmount;
        }
        public void SetStartDate(DateTime? startDate)
        {
            StartDate = startDate;
        }
        public void SetEndDate(DateTime? endDate)
        {
            EndDate = endDate;
        }
        public void SetInsuranceCompanyName(EnumInsuranceCompanyName insuranceCompanyName)
        {
            Check.Range((int)insuranceCompanyName, nameof(insuranceCompanyName), InsuranceConst.MinInsuranceCompanyName, InsuranceConst.MaxInsuranceCompanyName);
            InsuranceCompanyName = insuranceCompanyName;
        }
        public void SetDescription(string? description)
        {
            Description = description;
        }

    }
}
