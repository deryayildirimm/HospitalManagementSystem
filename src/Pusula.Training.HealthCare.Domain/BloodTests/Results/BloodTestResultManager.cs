using System;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.BloodTests.Tests;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.BloodTests.Results
{
    public class BloodTestResultManager(IBloodTestResultRepository bloodTestResultRepository,
        ITestRepository testRepository) : DomainService, IBloodTestResultManager
    {
        public virtual async Task<BloodTestResult> CreateAsync(Guid testId, double value)
        {
            Check.NotNullOrWhiteSpace(testId.ToString(), nameof(testId));
            Check.NotNullOrWhiteSpace(value.ToString(), nameof(value));

            var test = await testRepository.GetAsync(testId);

            var bloodResultStatus = value > test.MaxValue ? BloodResultStatus.High :
                                    value < test.MinValue ? BloodResultStatus.Low :
                                    BloodResultStatus.Normal;

            var result = new BloodTestResult(GuidGenerator.Create(), testId, value, bloodResultStatus);

            return await bloodTestResultRepository.InsertAsync(result);
        }

        public virtual async Task<BloodTestResult> UpdateAsync(Guid id, Guid testId, double value)
        {
            Check.NotNullOrWhiteSpace(testId.ToString(), nameof(testId));
            Check.NotNullOrWhiteSpace(value.ToString(), nameof(value));

            var result = await bloodTestResultRepository.GetWithNavigationPropertiesAsync(id);
            var test = await testRepository.GetAsync(testId);

            var bloodResultStatus = value > test.MaxValue ? BloodResultStatus.High :
                                    value < test.MinValue ? BloodResultStatus.Low :
                                    BloodResultStatus.Normal;

            result!.SetTestId(testId);
            result.SetValue(value);
            result.SetBloodResultStatus(bloodResultStatus);

            return await bloodTestResultRepository.UpdateAsync(result);
        }
    }
}
