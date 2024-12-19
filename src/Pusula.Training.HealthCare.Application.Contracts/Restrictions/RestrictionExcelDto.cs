namespace Pusula.Training.HealthCare.Restrictions;

public class RestrictionExcelDto
{
    public string DoctorName { get; set; } = null!;
    public string DepartmentName { get; set; } = null!;
    public string ServiceName { get; set; } = null!;
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public string? Gender { get; set; }
}