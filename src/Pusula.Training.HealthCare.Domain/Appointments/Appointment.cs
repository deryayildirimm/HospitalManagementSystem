using System;
using JetBrains.Annotations;
using Pusula.Training.HealthCare.AppointmentTypes;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Appointments;

public class Appointment : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public virtual Guid DoctorId { get; protected set; }

    [NotNull] 
    public virtual Guid PatientId { get; protected set; }

    [NotNull]
    public virtual Guid MedicalServiceId { get; protected set; }
    
    [NotNull]
    public virtual Guid AppointmentTypeId { get; protected set; }
    
    public virtual Doctor Doctor { get; protected set; }
    
    public virtual Patient Patient { get; protected set; }
    
    public virtual MedicalService MedicalService { get; protected set; }
    
    public virtual AppointmentType AppointmentType { get; protected set; }

    [NotNull]
    public virtual DateTime AppointmentDate { get; protected set; }
    
    [NotNull]
    public virtual DateTime StartTime { get; protected set; }

    [NotNull]
    public virtual DateTime EndTime { get; protected  set; } 

    [NotNull]
    public virtual EnumAppointmentStatus Status { get; protected set; }

    [CanBeNull]
    public virtual string? Notes { get; protected set; } = string.Empty;

    [NotNull]
    public virtual bool ReminderSent { get; protected set; }

    [NotNull] 
    public virtual double Amount { get; protected set; }

    protected Appointment()
    {
        Amount = 0.0;
        ReminderSent = false;
    }

    public Appointment(Guid id, Guid doctorId, Guid patientId, Guid medicalServiceId, Guid appointmentTypeId, DateTime appointmentDate,
        DateTime startTime, DateTime endTime, EnumAppointmentStatus status, string? notes, bool reminderSent, double amount)
    {
        Id = id;
        SetDoctorId(doctorId);
        SetPatientId(patientId);
        SetMedicalServiceId(medicalServiceId);
        SetAppointmentTypeId(appointmentTypeId);
        SetAppointmentDate(appointmentDate);
        SetStartTime(startTime);
        SetEndTime(endTime);
        SetStatus(status);
        SetNotes(notes);
        SetReminderSent(reminderSent);
        SetAmount(amount);
    }

    private void SetDoctorId(Guid doctorId)
    {
        Check.NotNull(doctorId, nameof(doctorId));
        DoctorId = doctorId;
    }

    private void SetPatientId(Guid patientId)
    {
        Check.NotNull(patientId, nameof(patientId));
        PatientId = patientId;
    }

    private void SetAppointmentTypeId(Guid appointmentTypeId)
    {
        Check.NotNull(appointmentTypeId, nameof(appointmentTypeId));
        AppointmentTypeId = appointmentTypeId;
    }
    
    private void SetMedicalServiceId(Guid medicalServiceId)
    {
        Check.NotNull(medicalServiceId, nameof(medicalServiceId));
        MedicalServiceId = medicalServiceId;
    }

    public void SetAppointmentDate(DateTime appointmentDate)
    {
        Check.NotNull(appointmentDate, nameof(appointmentDate));
        AppointmentDate = appointmentDate;
    }

    public void SetStartTime(DateTime startTime)
    {
        Check.NotNull(startTime, nameof(startTime));
        StartTime = startTime;
    }

    public void SetEndTime(DateTime endTime)
    {
        Check.NotNull(endTime, nameof(endTime));
        EndTime = endTime;
    }

    public void SetStatus(EnumAppointmentStatus status)
    {
        if (!Array.Exists(AppointmentConsts.ValidStatuses, s => s == status))
        {
            throw new ArgumentException($"Invalid appointment status.");
        }
        Status = status;
    }

    public void SetNotes(string? notes)
    {
        if (!string.IsNullOrEmpty(notes) && notes.Length > AppointmentConsts.MaxNotesLength)
        {
            throw new ArgumentException($"Notes length cannot exceed {AppointmentConsts.MaxNotesLength} characters.");
        }
        Notes = notes;
    }

    public void SetReminderSent(bool reminderSent)
    {
        Check.NotNull(reminderSent, nameof(reminderSent));
        ReminderSent = reminderSent;
    }

    public void SetAmount(double amount)
    {
        Check.NotNull(amount, nameof(amount));
        Check.Range(amount, nameof(amount), AppointmentConsts.MinAmount, AppointmentConsts.MaxAmount);
        Amount = amount;
    }
}