using System;
using System.Collections.Generic;
using System.Text;
using Pusula.Training.HealthCare.Localization;
using Volo.Abp.Application.Services;

namespace Pusula.Training.HealthCare;

/* Inherit your application services from this class.
 */
public abstract class HealthCareAppService : ApplicationService
{
    protected HealthCareAppService()
    {
        LocalizationResource = typeof(HealthCareResource);
    }
}
