using System;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.DoctorWorkingHours;

public interface IDoctorWorkingHourRepository : IRepository<DoctorWorkingHour, Guid>
{
    
}