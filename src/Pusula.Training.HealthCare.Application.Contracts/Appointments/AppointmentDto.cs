using System;
using Pusula.Training.HealthCare.AppointmentTypes;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentDto : FullAuditedEntityDto<Guid>
{

    public Guid DoctorId { get; set; }
    public Guid PatientId { get; set; }
    public Guid MedicalServiceId { get; set; }
    public Guid AppointmentTypeId { get; set; }
    public DoctorDto Doctor { get; set; } = null!;
    public PatientDto Patient { get; set; } = null!;
    public MedicalServiceDto MedicalService { get; set; } = null!;
    public DepartmentDto Department { get; set; } = null!;
    public AppointmentTypeDto AppointmentType { get; set; } = null!;
    public DateTime AppointmentDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public EnumAppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public bool ReminderSent { get; set; }
    public string? CancellationNotes { get; set; } = string.Empty;
    public double Amount { get; set; }
 
}