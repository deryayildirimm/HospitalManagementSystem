using Pusula.Training.HealthCare.Cities;

namespace Pusula.Training.HealthCare.Districts;

public class DistrictWithNavigationProperties
{
    public District District { get; set; } = null!;
    public City City { get; set; } = null!;
}