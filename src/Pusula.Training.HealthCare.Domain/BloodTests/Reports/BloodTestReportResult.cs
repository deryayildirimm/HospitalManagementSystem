using Pusula.Training.HealthCare.BloodTests.Results;
using System;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.BloodTests.Reports
{
    public class BloodTestReportResult : Entity
    {
        public Guid BloodTestReportId { get; set; }
        public BloodTestReport BloodTestReport { get; set; } = null!;
        public Guid BloodTestResultId { get; set; }
        public BloodTestResult BloodTestResult { get; set; } = null!;

        private BloodTestReportResult() { }

        public BloodTestReportResult(Guid bloodTestReportId, Guid bloodTestResultId)
        {
            BloodTestReportId = bloodTestReportId;
            BloodTestResultId = bloodTestResultId;
        }
        public override object?[] GetKeys() => new object[] { BloodTestReportId, BloodTestResultId };

    }
}
