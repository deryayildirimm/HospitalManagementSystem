using System;
using System.Collections.Generic;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class BloodTestExcelDto
    {
        public BloodTestStatus Status { get; set; }
        public DateTime dateCreated { get; set; } 
        public DateTime DateCompleted { get; set; }
        public string Doctor { get; set; } = null!;
        public string Patient { get; set; } = null!;
        public virtual List<BloodTestCategoryDto> BloodTestCategory { get; set; } = new List<BloodTestCategoryDto>();
    }
}
