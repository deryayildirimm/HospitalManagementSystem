using System;
using Pusula.Training.HealthCare.Patients;

namespace Pusula.Training.HealthCare.MedicalPersonnel;

public class MedicalStaffExcelDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string IdentityNumber { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public EnumGender Gender { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime StartDate { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string Department { get; set; }
}