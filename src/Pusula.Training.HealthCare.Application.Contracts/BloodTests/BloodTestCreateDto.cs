using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class BloodTestCreateDto
    {
        [Required]
        public BloodTestStatus Status { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        public DateTime DateCompleted { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public Guid TestCategoryId { get; set; }
    }
}
