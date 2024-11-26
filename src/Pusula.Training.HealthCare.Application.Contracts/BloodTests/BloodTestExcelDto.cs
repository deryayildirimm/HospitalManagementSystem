using System;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class BloodTestExcelDto
    {
        public BloodTestStatus Status { get; set; }
        public DateTime dateCreated { get; set; } 
        public DateTime DateCompleted { get; set; } 
        public string Doctor { get; set; }
        public string Patient { get; set; }
        public string TestCategory { get; set; }
    }
}
