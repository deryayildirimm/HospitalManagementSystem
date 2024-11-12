using System;
using Pusula.Training.HealthCare.Patients;

namespace Pusula.Training.HealthCare.Doctors;

public class DoctorExcelDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string IdentityNumber { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public EnumGender Gender { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public int YearOfExperience { get; set; } = 0;
    public string City { get; set; } = null!;
    public string District { get; set; } = null!;
    public string Department { get; set; }
    public string Title { get; set; }
}