using Volo.Abp.Settings;

namespace Pusula.Training.HealthCare.Settings;

public class HealthCareSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(HealthCareSettings.MySetting1));
    }
}
