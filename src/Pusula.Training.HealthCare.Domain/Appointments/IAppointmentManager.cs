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
    
}