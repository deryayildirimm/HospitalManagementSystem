using System;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.AppointmentTypes;

public interface IAppointmentTypeManager
{
    Task<AppointmentType> CreateAsync(
        string name
    );
    
    Task<AppointmentType> UpdateAsync(
        Guid id,
        string name,
        string? concurrencyStamp = null
    );
}