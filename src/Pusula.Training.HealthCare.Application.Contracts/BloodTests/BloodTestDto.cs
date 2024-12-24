using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Patients;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class BloodTestDto : AuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        [Required]
        public BloodTestStatus Status { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        public Guid DoctorId { get; set; }
        [Required]
        public Guid PatientId { get; set; }
        public DateTime DateCompleted { get; set; }
        public DoctorDto Doctor { get; set; } = null!;
        public PatientDto Patient { get; set; } = null!;
        public List<BloodTestCategoryDto>? BloodTestCategories { get; set; } 

        public string ConcurrencyStamp { get; set; } = null!;
    }
}
