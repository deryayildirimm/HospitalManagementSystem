using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Appointments;

namespace Pusula.Training.HealthCare.MedicalServices;

public interface IMedicalServiceManager
{
    Task<MedicalService> CreateAsync(
        string name, 
        DateTime serviceCreatedAt, 
        double cost, 
        int duration,
        List<string> departmentNames
    );
    
    Task<MedicalService> UpdateAsync(
        Guid id,
        string name,
        double cost,
        int duration,
        DateTime serviceCreatedAt,
        List<string> departmentNames,
        string? concurrencyStamp = null
    );
}