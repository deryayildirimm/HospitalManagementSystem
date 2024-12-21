using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Patients
{
    public interface IPatientManager : IDomainService
    {
        Task<Patient> CreateAsync(int patientNumber, string firstName, string lastName, DateTime birthDate, EnumGender gender, string identityNumber, string passportNumber,
        string? nationality = null, string? mobilePhoneNumber = null, EnumPatientTypes? patientType = null, string? mothersName = null, string? fathersName = null,
        string? emailAddress = null, EnumRelative? relative = null, string? relativePhoneNumber = null, string? address = null, EnumDiscountGroup? discountGroup = null);

        Task<Patient> UpdateAsync(Guid id, string firstName, string lastName, DateTime birthDate, EnumGender gender, string identityNumber, string? passportNumber, 
            string? nationality = null, string? mobilePhoneNumber = null, EnumPatientTypes? patientType = null, string? mothersName = null, string? fathersName = null, 
            string? emailAddress = null, EnumRelative? relative = null, string? relativePhoneNumber = null, string? address = null, EnumDiscountGroup? discountGroup = null, 
            string? concurrencyStamp = null);
    }
}
