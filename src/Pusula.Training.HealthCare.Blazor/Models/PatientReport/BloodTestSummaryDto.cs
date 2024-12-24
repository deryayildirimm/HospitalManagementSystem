using Pusula.Training.HealthCare.BloodTests;
using System;

namespace Pusula.Training.HealthCare.Blazor.Models.PatientReport
{
    public class BloodTestSummaryDto
    {
        public Guid BloodTestId { get; set; }
        public DateTime DateCreated { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public BloodTestStatus Status { get; set; }
    }
}
