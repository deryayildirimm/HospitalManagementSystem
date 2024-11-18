using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Appointments;

public interface IAppointmentRepository : IRepository<Appointment, Guid>
{
    
}