using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Pusula.Training.HealthCare.Departments;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.AppointmentTypes;

public class AppointmentTypeManager(
    IAppointmentTypeRepository appointmentTypeRepository)
    : DomainService, IAppointmentTypeManager
{
    public virtual async Task<AppointmentType> CreateAsync(
        string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.Length(name, nameof(name), DepartmentConsts.NameMaxLength);

        var appointmentType = new AppointmentType(
            GuidGenerator.Create(),
            name
        );

        return await appointmentTypeRepository.InsertAsync(appointmentType);
    }

    public virtual async Task<AppointmentType> UpdateAsync(
        Guid id,
        string name, [CanBeNull] string? concurrencyStamp = null
    )
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.Length(name, nameof(name), DepartmentConsts.NameMaxLength);

        var appointmentType = await appointmentTypeRepository.GetAsync(id);

        appointmentType.SetName(name);

        appointmentType.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await appointmentTypeRepository.UpdateAsync(appointmentType);
    }
}