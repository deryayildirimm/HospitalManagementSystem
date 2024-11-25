using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using static Pusula.Training.HealthCare.Permissions.HealthCarePermissions;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class BloodTestResultManager(IBloodTestResultRepository bloodTestResultRepository) : DomainService, IBloodTestResultManager
    {
        public virtual async Task<BloodTestResult> CreateAsync(Guid bloodTestId, Guid testId, double value, BloodResultStatus bloodResultStatus)
        {
            Check.NotNullOrWhiteSpace(bloodTestId.ToString(), nameof(bloodTestId));
            Check.NotNullOrWhiteSpace(testId.ToString(), nameof(testId));
            Check.NotNullOrWhiteSpace(value.ToString(), nameof(value));
            Check.Range((int)bloodResultStatus, nameof(bloodResultStatus), BloodTestConst.BloodTestStatusMin, BloodTestConst.BloodTestStatusMax);

            var result = new BloodTestResult(GuidGenerator.Create(), bloodTestId, testId, value, bloodResultStatus);

            return await bloodTestResultRepository.InsertAsync(result);
        }
    }
}
