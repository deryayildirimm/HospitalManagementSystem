using System.Collections.Generic;
using System;
using Pusula.Training.HealthCare.BloodTests;

namespace Pusula.Training.HealthCare.Blazor.Models.LaboratoryTechnicianPage
{
    public class BloodTestViewModel
    {
        public Guid BloodTestId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientLastName { get; set; } = string.Empty;
        public int PatientNumber { get; set; }
        public string PatientID { get; set; } = string.Empty;
        public BloodTestStatus BloodTestStatus { get; set; }
        public DateTime BloodTestDateCreated { get; set; }
        public DateTime BloodTestDateCompleted { get; set; }
        public List<Guid> TestCategoryIds { get; set; } = new List<Guid>();
    }
}
