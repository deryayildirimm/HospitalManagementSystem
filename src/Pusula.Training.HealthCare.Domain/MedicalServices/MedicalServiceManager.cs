using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Exceptions;
using Pusula.Training.HealthCare.GlobalExceptions;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.MedicalServices;

public class MedicalServiceManager(
    IMedicalServiceRepository medicalServiceRepository,
    IDepartmentRepository departmentRepository) : DomainService, IMedicalServiceManager
{
    public virtual async Task<MedicalService> CreateAsync(
        string name,
        DateTime serviceCreatedAt,
        double cost,
        int duration,
        List<string> departmentNames)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.NotNull(serviceCreatedAt, nameof(serviceCreatedAt));
        Check.Range(cost, nameof(cost), MedicalServiceConsts.CostMinValue);
        Check.Range(duration, nameof(duration), MedicalServiceConsts.DurationMinValue,
            MedicalServiceConsts.DurationMaxValue);
        Check.NotNull(departmentNames, nameof(departmentNames));

        var medicalService = new MedicalService(
            GuidGenerator.Create(),
            name,
            cost,
            duration,
            serviceCreatedAt
        );

        await SetDepartmentsAsync(service: medicalService, departmentNames: departmentNames);
        return await medicalServiceRepository.InsertAsync(medicalService);
    }

    public virtual async Task<MedicalService> UpdateAsync(
        Guid id,
        string name,
        double cost,
        int duration,
        DateTime serviceCreatedAt,
        List<string> departmentNames,
        string? concurrencyStamp = null
    )
    {
        Check.NotNull(id, nameof(id));
        Check.NotNull(serviceCreatedAt, nameof(serviceCreatedAt));
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.Range(cost, nameof(cost), MedicalServiceConsts.CostMinValue);
        Check.NotNull(departmentNames, nameof(departmentNames));
        Check.Range(duration, nameof(duration), MedicalServiceConsts.DurationMinValue,
            MedicalServiceConsts.DurationMaxValue);

        var service = await medicalServiceRepository.FirstOrDefaultAsync(x => x.Id == id);

        HealthcareGlobalException.ThrowIf("MedicalService not found.",HealthCareDomainErrorCodes.MedicalServiceNotFound, service is null);
        
        service.SetName(name);
        service.SetCost(cost);
        service.SetDuration(duration);
        service.SetServiceCreatedAt(serviceCreatedAt);
        service.SetConcurrencyStampIfNotNull(concurrencyStamp);

        await SetDepartmentsAsync(service: service, departmentNames: departmentNames);
        return await medicalServiceRepository.UpdateAsync(service);
    }

    protected virtual async Task SetDepartmentsAsync(MedicalService service, List<string> departmentNames)
    {
        if (departmentNames.Count == 0)
        {
            return;
        }

        var departmentIds = (await departmentRepository.GetListByNamesAsync(departmentNames.ToArray()))
            .Select(x => x.Id)
            .ToList();
        
        HealthcareGlobalException.ThrowIf("Departments not found.",HealthCareDomainErrorCodes.DepartmentsNotFound, departmentIds.Count == 0);

        foreach (var id in departmentIds)
        {
            service.AddDepartment(id);
        }
    }
}