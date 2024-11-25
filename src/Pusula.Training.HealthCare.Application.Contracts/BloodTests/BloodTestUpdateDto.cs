using System;
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
        public DateTime DateCompleted { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public Guid TestCategoryId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}
