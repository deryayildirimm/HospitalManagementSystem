using Pusula.Training.HealthCare.BloodTests.Category;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Titles;
using System;
using System.Collections.Generic;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class GroupedBloodTestDto
    {
        public PatientDto Patient { get; set; } = null!;
        public DoctorDto Doctor { get; set; } = null!;
        public DepartmentDto Department { get; set; } = null!;
        public TitleDto Title { get; set; } = null!;
        public DateTime DateRequested { get; set; }
        public List<TestCategoryDto> SelectedCategories { get; set; } = new();
        public List<Guid> TestIds { get; set; } = new();
        public string Status { get; set; } = string.Empty; 


    }
}
