using Pusula.Training.HealthCare.Cities;

namespace Pusula.Training.HealthCare.Districts;

public class DistrictWithNavigationPropertiesDto
{
    public DistrictDto District { get; set; } = null!;
    public CityDto City { get; set; } = null!;
}