using Pusula.Training.HealthCare.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Pusula.Training.HealthCare.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class HealthCareController : AbpControllerBase
{
    protected HealthCareController()
    {
        LocalizationResource = typeof(HealthCareResource);
    }
}
