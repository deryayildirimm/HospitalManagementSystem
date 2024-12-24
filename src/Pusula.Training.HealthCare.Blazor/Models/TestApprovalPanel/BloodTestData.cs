using Pusula.Training.HealthCare.BloodTests;
using System;

namespace Pusula.Training.HealthCare.Blazor.Models.TestApprovalPanel
{
    public class BloodTestData
    {
        public Guid BloodTestId { get; set; }
        public string PatientId { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public int PatientNumber { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public BloodTestStatus Status { get; set; }
    }
}
