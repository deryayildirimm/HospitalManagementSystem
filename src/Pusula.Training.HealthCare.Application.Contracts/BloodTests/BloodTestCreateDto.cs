using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class BloodTestCreateDto
    {
        [Required]
        public BloodTestStatus Status { get; set; }
        [Required]
        public virtual DateTime DateCreated { get; set; }
        [Required]
        public virtual Guid DoctorId { get; set; }
        [Required]
        public virtual Guid PatientId { get; set; }
        public virtual DateTime DateCompleted { get; set; }
        public virtual List<Guid> TestCategoryIdList { get; set; } = new List<Guid>();
    }
}
