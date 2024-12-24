using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class BloodTestUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public Guid Id { get; set; } = default!;  
        [Required]
        public BloodTestStatus Status { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        public Guid DoctorId { get; set; }
        [Required]
        public Guid PatientId { get; set; }
        public DateTime DateCompleted { get; set; }
        public virtual List<Guid> TestCategoryIdList { get; set; } = new List<Guid>();

        public string ConcurrencyStamp { get; set; } = null!;
    }
}
