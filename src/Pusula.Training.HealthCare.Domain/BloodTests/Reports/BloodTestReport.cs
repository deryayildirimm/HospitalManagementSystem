using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.BloodTests.Reports
{
    public class BloodTestReport : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public Guid BloodTestId { get; private set; }
        public BloodTest BloodTest { get; private set; } = null!;
        public ICollection<BloodTestReportResult> Results { get; private set; }

        public BloodTestReport()
        {
            BloodTestId = Guid.Empty;
            Results = new List<BloodTestReportResult>();    
        }

        public BloodTestReport(Guid id, Guid bloodTestId) : this()
        {
            Id = id;
            SetBloodTestId(bloodTestId);  
        }

        public void SetBloodTestId(Guid bloodTestId)
        {
            Check.NotNullOrWhiteSpace(bloodTestId.ToString(), nameof(bloodTestId));
            BloodTestId = bloodTestId;
        }

        public void AddResult(Guid testResultId)
        {
            Check.NotNull(testResultId, nameof(testResultId));

            if (IsInCategory(testResultId))
            {
                return;
            }

            Results.Add(new BloodTestReportResult(bloodTestReportId: Id, testResultId));
        }

        public void RemoveResult(Guid testResultId)
        {
            Check.NotNull(testResultId, nameof(testResultId));

            if (!IsInCategory(testResultId))
            {
                return;
            }

            Results.RemoveAll(x => x.BloodTestResultId == testResultId);
        }

        public void RemoveAllResultsExceptGivenIds(List<Guid> testResultIds)
        {
            Check.NotNullOrEmpty(testResultIds, nameof(testResultIds));

            Results.RemoveAll(x => !testResultIds.Contains(x.BloodTestResultId));
        }

        public void RemoveAllResults()
        {
            Results.RemoveAll(x => x.BloodTestReportId == Id);
        }

        private bool IsInCategory(Guid testResultId)
        {
            return Results.Any(x => x.BloodTestResultId == testResultId);
        }
    }
}
