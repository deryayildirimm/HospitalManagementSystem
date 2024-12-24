using Pusula.Training.HealthCare.BloodTests.Results;
using System;
using System.Text.Json.Serialization;

namespace Pusula.Training.HealthCare.BloodTests.Reports
{
    public class BloodTestReportResultDto
    {
        public Guid BloodTestReportId { get; set; }
        [JsonIgnore]
        public BloodTestReportDto BloodTestReport { get; set; } = null!;

        public Guid BloodTestResultId { get; set; }
        public BloodTestResultDto BloodTestResult { get; set; } = null!;
    }
}
