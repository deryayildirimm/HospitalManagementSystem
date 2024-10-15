using Pusula.Training.HealthCare.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Pusula.Training.HealthCare.Blazor;

public abstract class HealthCareComponentBase : AbpComponentBase
{
    protected HealthCareComponentBase()
    {
        LocalizationResource = typeof(HealthCareResource);
    }
}
