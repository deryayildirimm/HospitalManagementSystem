using System;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Insurances;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.ProtocolTypes;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Protocols
{
    public class ProtocolUpdateDto : IHasConcurrencyStamp
    {
       
       
        public string? Notes { get; set; } 
        [Required] public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Guid PatientId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid ProtocolTypeId { get; set; }
        public Guid DoctorId { get; set; }
        
        public Guid InsuranceId { get; set; }
        
        public PatientDto Patient { get; set; } 
        public DepartmentDto Department { get; set; } 
        public ProtocolTypeDto ProtocolType { get; set; } 
        public DoctorDto Doctor { get; set; } 
        public InsuranceDto Insurance { get; set; }
        public string ConcurrencyStamp { get; set; } = null!;
    }
}