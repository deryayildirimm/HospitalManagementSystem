using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.BloodTests.Reports
{
    public class BloodTestReportManager(IBloodTestReportRepository bloodTestReportRepository) : DomainService, IBloodTestReportManager
    {
        public virtual async Task<BloodTestReport> CreateAsync(Guid bloodTestId, List<Guid>? bloodTestResultIds = null)
        {
            Check.NotNullOrWhiteSpace(bloodTestId.ToString(), nameof(bloodTestId));

            var bloodTestReport = new BloodTestReport(GuidGenerator.Create(), bloodTestId);

            await SetResultsAsync(bloodTestReport, bloodTestResultIds!);

            await bloodTestReportRepository.InsertAsync(bloodTestReport,true);

            return await bloodTestReportRepository.GetAsync(bloodTestReport.Id);
        }

        public virtual async Task<BloodTestReport> UpdateAsync(Guid id, Guid bloodTestId, List<Guid>? bloodTestResultIds = null)
        {
            Check.NotNullOrWhiteSpace(bloodTestId.ToString(), nameof(bloodTestId));

            var bloodTestReport = await bloodTestReportRepository.GetWithNavigationPropertiesAsync(id);

            bloodTestReport!.SetBloodTestId(bloodTestId);

            await SetResultsAsync(bloodTestReport, bloodTestResultIds!);

            return await bloodTestReportRepository.UpdateAsync(bloodTestReport);
        }

        private Task SetResultsAsync(BloodTestReport bloodTestReport, [CanBeNull] List<Guid> resultIds)
        {
            if (resultIds?.Any() != true)
            {
                bloodTestReport.RemoveAllResults();
                return Task.CompletedTask;
            }

            bloodTestReport.RemoveAllResultsExceptGivenIds(resultIds);

            foreach (var resultId in resultIds)
            {
                bloodTestReport.AddResult(resultId);
            }

            return Task.CompletedTask;
        }
    }
}
