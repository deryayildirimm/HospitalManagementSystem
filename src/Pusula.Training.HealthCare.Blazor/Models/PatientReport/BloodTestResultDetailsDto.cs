using Pusula.Training.HealthCare.BloodTests;
using System;

namespace Pusula.Training.HealthCare.Blazor.Models.PatientReport
{
    public class BloodTestResultDetailsDto
    {
        public Guid BloodTestId { get; set; }
        public Guid PatientId { get; set; }
        public Guid TestId { get; set; }
        public double Value { get; set; }
        public BloodResultStatus BloodResultStatus { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string? Interval { get; set; }
    }
}
