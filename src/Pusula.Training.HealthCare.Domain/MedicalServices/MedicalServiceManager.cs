using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Departments;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.MedicalServices;

public class MedicalServiceManager(
    IMedicalServiceRepository medicalServiceRepository,
    IDepartmentRepository departmentRepository) : DomainService
{
    public virtual async Task<MedicalService> CreateAsync(
        string name, DateTime serviceCreatedAt, double cost, List<string> departmentNames)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.NotNull(serviceCreatedAt, nameof(serviceCreatedAt));
        Check.Range(cost, nameof(cost), MedicalServiceConsts.CostMinValue);
        Check.NotNull(departmentNames, nameof(departmentNames));

        var medicalService = new MedicalService(
            GuidGenerator.Create(),
            name,
            cost,
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

        var service = await medicalServiceRepository.GetAsync(id);

        if (service == null)
        {
            throw new UserFriendlyException($"Medical service with was not found.");
        }

        service.Name = name;
        service.Cost = cost;
        service.ServiceCreatedAt = serviceCreatedAt;
        service.SetConcurrencyStampIfNotNull(concurrencyStamp);

        var departments = await departmentRepository
            .GetListByNamesAsync(departmentNames.ToArray());

        if (departments != null && departments.Count != 0)
        {
            foreach (var department in departments)
            {
                service.DepartmentMedicalServices.Add(new DepartmentMedicalService
                {
                    DepartmentId = department.Id,
                    MedicalServiceId = service.Id,
                });
            }
        }

        return await medicalServiceRepository.UpdateAsync(service);
    }
}