using System;
using System.ComponentModel.DataAnnotations;


namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class BloodTestResultCreateDto
    {
        [Required]
        public virtual double Value { get; set; } 
        [Required]
        public virtual BloodResultStatus BloodResultStatus { get; set; }
        public virtual Guid BloodTestId { get; set; }
        public virtual Guid TestId { get; set; }
    }
}
