using Microsoft.Extensions.Localization;
using Pusula.Training.HealthCare.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Pusula.Training.HealthCare.Blazor;

[Dependency(ReplaceServices = true)]
public class HealthCareBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<HealthCareResource> _localizer;

    public HealthCareBrandingProvider(IStringLocalizer<HealthCareResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
