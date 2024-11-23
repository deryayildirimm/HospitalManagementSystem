using Microsoft.Extensions.Localization;
using Pusula.Training.HealthCare.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Pusula.Training.HealthCare.Blazor;

[Dependency(ReplaceServices = true)]
public class HealthCareBrandingProvider(IStringLocalizer<HealthCareResource> localizer) : DefaultBrandingProvider
{
    public override string AppName => localizer["AppName"];

    public override string LogoUrl => "images/logo/company/logo.png";
}
