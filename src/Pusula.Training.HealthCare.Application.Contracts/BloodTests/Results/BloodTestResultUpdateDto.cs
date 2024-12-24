using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.BloodTests.Results
{
    public class BloodTestResultUpdateDto
    {
        [Required]
        public Guid Id { get; set; } = default!;
        [Required]
        public virtual double Value { get; set; }
        public virtual BloodResultStatus BloodResultStatus { get; set; }
        [Required]
        public virtual Guid TestId { get; set; }
    }
}
