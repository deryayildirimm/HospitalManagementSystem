using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.BloodTests.Reports
{
    public class BloodTestReportCreateDto
    {
        [Required]
        public Guid BloodTestId { get; set; }
        public virtual List<Guid> BloodTestResultIds { get; set; } = new List<Guid>();
    }
}
