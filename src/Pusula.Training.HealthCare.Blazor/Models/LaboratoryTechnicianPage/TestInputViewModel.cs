using Pusula.Training.HealthCare.BloodTests;
using System;

namespace Pusula.Training.HealthCare.Blazor.Models.LaboratoryTechnicianPage
{
    public class TestInputViewModel
    {
        public Guid TestId { get; set; }
        public Guid? ResultId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double? Value { get; set; }
    }
}
