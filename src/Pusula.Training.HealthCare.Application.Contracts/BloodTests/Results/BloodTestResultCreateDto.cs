using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.BloodTests.Results
{
    public class BloodTestResultCreateDto
    {
        [Required]
        public virtual double Value { get; set; }
        public virtual BloodResultStatus BloodResultStatus { get; set; }
        [Required]
        public virtual Guid TestId { get; set; }
    }
}
