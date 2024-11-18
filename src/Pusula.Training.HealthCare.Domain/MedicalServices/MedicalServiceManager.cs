using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Departments;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.MedicalServices;

public class MedicalServiceManager(
    IMedicalServiceRepository medicalServiceRepository,
    IDepartmentRepository departmentRepository) : DomainService
{
    public virtual async Task<MedicalService> CreateAsync(
        string name, DateTime serviceCreatedAt, double cost, int duration, List<string> departmentNames)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.NotNull(serviceCreatedAt, nameof(serviceCreatedAt));
        Check.Range(cost, nameof(cost), MedicalServiceConsts.CostMinValue);
        Check.Range(duration, nameof(duration), MedicalServiceConsts.DurationMinValue, MedicalServiceConsts.DurationMaxValue);
        Check.NotNull(departmentNames, nameof(departmentNames));

        var medicalService = new MedicalService(
            GuidGenerator.Create(),
            name,
            cost,
            duration,
            serviceCreatedAt
        );

        var departments = await departmentRepository.GetListByNamesAsync(departmentNames.ToArray());

        if (departments == null || departments.Count == 0)
        {
            throw new UserFriendlyException("Invalid departments.");
        }

        foreach (var department in departments)
        {
            var departmentMedicalService = new DepartmentMedicalService
            {
                DepartmentId = department.Id,
                MedicalServiceId = medicalService.Id,
            };
            medicalService.DepartmentMedicalServices.Add(departmentMedicalService);
        }

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
        Check.Range(duration, nameof(duration), MedicalServiceConsts.DurationMinValue, MedicalServiceConsts.DurationMaxValue);
        
        var service = await medicalServiceRepository.GetAsync(id);

        if (service == null)
        {
            throw new UserFriendlyException($"Medical service with was not found.");
        }

        service.Name = name;
        service.Cost = cost;
        service.Duration = duration;
        service.ServiceCreatedAt = serviceCreatedAt;
        service.SetConcurrencyStampIfNotNull(concurrencyStamp);

        var departments = await departmentRepository
            .GetListByNamesAsync(departmentNames.ToArray());

        if (departments.Count == 0)
        {
            return await medicalServiceRepository.UpdateAsync(service);
        }

        foreach (var department in departments)
        {
            service.DepartmentMedicalServices.Add(new DepartmentMedicalService
            {
                DepartmentId = department.Id,
                MedicalServiceId = service.Id,
            });
        }
        
        return await medicalServiceRepository.UpdateAsync(service);

    }
}