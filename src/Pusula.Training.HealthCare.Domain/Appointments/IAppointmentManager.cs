using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Appointments;

public interface IAppointmentManager
{
    Task<List<AppointmentSlotDto>> GetAppointmentSlotsAsync(
        Guid doctorId,
        Guid medicalServiceId,
        DateTime date
    );

    Task<Appointment> CreateAsync(
        Guid doctorId, 
        Guid patientId, 
        Guid medicalServiceId, 
        DateTime appointmentDate,
        DateTime startTime,
        DateTime endTime,
        bool reminderSent,
        double amount,
        string? notes = null
    );
    
    Task<Appointment> UpdateAsync(
        Guid id,
        DateTime appointmentDate,
        DateTime startTime,
        DateTime endTime,
        EnumAppointmentStatus status,
        bool reminderSent,
        double amount,
        string? notes = null,
        string? concurrencyStamp = null
    );
    
}